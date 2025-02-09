namespace Belin.Cli.CommandLine;

using MySqlConnector;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// Provides extension methods for commands.
/// </summary>
public static class CommandExtensions {

	/// <summary>
	/// The list of local host names.
	/// </summary>
	private static readonly string[] localHosts = ["::1", "127.0.0.1", "localhost"];

	/// <summary>
	/// Checks whether this command should be executed in an elevated prompt.
	/// </summary>
	/// <param name="_">The current command.</param>
	/// <param name="output">The path to the output directory.</param>
	/// <returns><see langword="true"/> if this command should be executed in an elevated prompt, otherwise <see langword="false"/>.</returns>
	public static bool CheckPrivilege(this Command _, DirectoryInfo? output = null) {
		var isPrivileged = Environment.IsPrivilegedProcess;
		if (output is not null) {
			var home = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
			if (output.Root.Name != home.Root.Name || output.FullName.StartsWith(home.FullName)) isPrivileged = true;
		}

		if (!isPrivileged) Console.WriteLine("You must run this command in an elevated prompt.");
		return isPrivileged;
	}

	/// <summary>
	/// Creates a new database connection.
	/// </summary>
	/// <param name="_">The current command.</param>
	/// <param name="uri">The connection string used to connect to the database.</param>
	/// <returns>The newly created database connection.</returns>
	public static async Task<MySqlConnection> CreateDbConnection(this Command _, Uri uri) {
		var userInfo = uri.UserInfo.Split(':').Select(Uri.UnescapeDataString).ToArray();
		var builder = new MySqlConnectionStringBuilder {
			Server = uri.Host,
			Port = uri.IsDefaultPort ? 3306 : (uint) uri.Port,
			Database = "information_schema",
			UserID = userInfo[0],
			Password = userInfo[1],
			Pooling = false,
			UseCompression = localHosts.Contains(uri.Host)
		};

		var connection = new MySqlConnection(builder.ConnectionString);
		await connection.OpenAsync();
		return connection;
	}

	/// <summary>
	/// Creates a new HTTP client.
	/// </summary>
	/// <param name="_">The current command.</param>
	/// <returns>The newly created HTTP client.</returns>
	public static HttpClient CreateHttpClient(this Command _) {
		var fileVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>();
		var userAgent = $".NET/{Environment.Version} | Belin.io/{new Version(fileVersion!.Version).ToString(3)}";
		var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("user-agent", userAgent);
		return httpClient;
	}

	/// <summary>
	/// Extracts the specified ZIP file into a given directory.
	/// </summary>
	/// <param name="_">The current command.</param>
	/// <param name="input">The path to the input ZIP file.</param>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="strip">The number of leading directory components to remove from file names on extraction.</param>
	public static void ExtractZipFile(this Command _, FileInfo input, DirectoryInfo output, int strip = 0) {
		Console.WriteLine($"Extracting file \"{input.Name}\" into directory \"{output.FullName}\"...");

		using var zipArchive = ZipFile.OpenRead(input.FullName);
		if (strip == 0) zipArchive.ExtractToDirectory(output.FullName, overwriteFiles: true);
		else foreach (var entry in zipArchive.Entries) {
			var path = string.Join('/', entry.FullName.Split('/').Skip(strip));
			if (path.Length == 0) path = "/";

			var fullPath = Path.Join(output.FullName, path);
			if (fullPath[^1] == '/') Directory.CreateDirectory(fullPath);
			else entry.ExtractToFile(fullPath, overwrite: true);
		}
	}

	/// <summary>
	/// Runs the specified executable with the <c>--version</c> argument.
	/// </summary>
	/// <param name="_">The current command.</param>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="executable">The executable path, relative to the output directory.</param>
	/// <returns>The standard output of the underlying process.</returns>
	/// <exception cref="Exception">An error occurred when starting the underlying process.</exception>
	public static string GetExecutableVersion(this Command _, DirectoryInfo output, string executable) {
		// TODO inutile de capturer la sortie standard!!! par défault, elle est affichée à l'écran !!!
		var startInfo = new ProcessStartInfo {
			Arguments = "--version",
			CreateNoWindow = true,
			FileName = Path.Join(output.FullName, executable),
			RedirectStandardOutput = true
		};

		// TODO remove exceptions and use nullables instead.
		using var process = Process.Start(startInfo) ?? throw new Exception($"The \"{executable}\" process could not be started.");
		var standardOutput = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();
		if (process.ExitCode != 0) throw new Exception($"The \"{executable}\" process failed with exit code {process.ExitCode}.");
		return standardOutput;
	}
}

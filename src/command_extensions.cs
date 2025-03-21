namespace Belin.Cli;

using MySqlConnector;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;

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
	/// Creates a new HTTP client.
	/// </summary>
	/// <param name="_">The current command.</param>
	/// <returns>The newly created HTTP client.</returns>
	public static HttpClient CreateHttpClient(this Command _) {
		var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(".NET", Environment.Version.ToString(3)));
		return httpClient;
	}

	/// <summary>
	/// Creates a new database connection.
	/// </summary>
	/// <param name="_">The current command.</param>
	/// <param name="uri">The connection string used to connect to the database.</param>
	/// <returns>The newly created database connection.</returns>
	public static MySqlConnection CreateMySqlConnection(this Command _, Uri uri) {
		var userInfo = uri.UserInfo.Split(':').Select(Uri.UnescapeDataString);
		var builder = new MySqlConnectionStringBuilder {
			Server = uri.Host,
			Port = uri.IsDefaultPort ? 3306 : (uint) uri.Port,
			Database = "information_schema",
			UserID = userInfo.First(),
			Password = userInfo.Last(),
			ConvertZeroDateTime = true,
			Pooling = false,
			UseCompression = !localHosts.Contains(uri.Host)
		};

		var connection = new MySqlConnection(builder.ConnectionString);
		connection.Open();
		return connection;
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
	/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
	public static string GetExecutableVersion(this Command _, DirectoryInfo output, string executable) {
		var startInfo = new ProcessStartInfo(Path.Join(output.FullName, executable), "--version") {
			CreateNoWindow = true,
			RedirectStandardOutput = true
		};

		using var process = Process.Start(startInfo) ?? throw new ProcessException(startInfo.FileName);
		var standardOutput = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();
		if (process.ExitCode != 0) throw new ProcessException(startInfo.FileName, $"The process failed with exit code {process.ExitCode}.");
		return standardOutput;
	}

	/// <summary>
	/// Parses the specified query string into a name-value collection.
	/// </summary>
	/// <param name="_">The current command.</param>
	/// <param name="query">The query string.</param>
	/// <returns>The name-value collection corresponding to the specified query string.</returns>
	public static NameValueCollection ParseQueryString(this Command _, string query) {
		if (query.StartsWith('?')) query = query[1..];

		var collection = new NameValueCollection();
		if (query.Length > 0) foreach (var param in query.Split('&')) {
			var parts = param.Split('=');
			if (parts.Length > 0) collection.Add(Uri.UnescapeDataString(parts[0]), parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : null);
		}

		return collection;
	}
}

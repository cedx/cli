namespace Belin.Cli.CommandLine;

using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

/// <summary>
/// Provides extension methods for commands.
/// </summary>
public static class Extensions {

	/// <summary>
	/// Checks whether this command should be executed in an elevated prompt.
	/// </summary>
	/// <param name="_">The current command.</param>
	/// <param name="output">The path to the output directory.</param>
	/// <returns><see langword="true"/> if this command should be executed in an elevated prompt, otherwise <see langword="false"/>.</returns>
	public static bool CheckPrivilege(this Command _, DirectoryInfo? output = null) {
		var isPrivileged = Environment.IsPrivilegedProcess;
		if (output != null) {
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
		zipArchive.ExtractToDirectory(output.FullName, overwriteFiles: true);
		// TODO Implements the `strip` of components!
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

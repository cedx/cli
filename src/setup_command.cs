namespace Belin.Cli;

using Belin.Cli.Setup;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;

/// <summary>
/// Downloads and installs a runtime environment.
/// </summary>
public sealed class SetupCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public SetupCommand(): base("setup", "Download and install a runtime environment.") {
		Add(new JdkCommand());
		Add(new NodeCommand());
		Add(new PhpCommand());
	}

	/// <summary>
	/// Creates a new HTTP client.
	/// </summary>
	/// <returns>The newly created HTTP client.</returns>
	internal static HttpClient CreateHttpClient() {
		var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(".NET", Environment.Version.ToString(3)));
		return httpClient;
	}

	/// <summary>
	/// Extracts the specified ZIP file into a given directory.
	/// </summary>
	/// <param name="input">The path to the input ZIP file.</param>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="strip">The number of leading directory components to remove from file names on extraction.</param>
	internal static void ExtractZipFile(FileInfo input, DirectoryInfo output, int strip = 0) {
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
	/// <param name="output">The path to the output directory.</param>
	/// <param name="executable">The executable path, relative to the output directory.</param>
	/// <returns>The standard output of the underlying process.</returns>
	/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
	internal static string GetExecutableVersion(DirectoryInfo output, string executable) {
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
}

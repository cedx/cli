namespace Belin.Cli.Commands;

using System.Diagnostics;
using System.IO.Compression;

/// <summary>
/// Downloads and installs the latest OpenJDK release.
/// </summary>
public class JdkCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public JdkCommand(): base("jdk", "Download and install the latest OpenJDK release.") {
		var javaOption = new Option<int>(["-j", "--java"], () => 21, "The major version of the Java development kit.") { ArgumentHelpName = "version" };
		var outputOption = new OutputOption(new DirectoryInfo(@"C:\Program Files\OpenJDK"));
		Add(javaOption);
		Add(outputOption);
		this.SetHandler(Execute, javaOption.FromAmong(["21", "17", "11", "8"]), outputOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	private async Task Execute(int java, DirectoryInfo output) {
		using var httpClient = new HttpClient();
		var path = await DownloadArchive(httpClient, java);
		ExtractArchive(path, output);

		var startInfo = new ProcessStartInfo("bin/java", ["--version"]) { CreateNoWindow = true, RedirectStandardOutput = true, WorkingDirectory = output.FullName };
		using var process = Process.Start(startInfo) ?? throw new Exception(@"The ""java --version"" process could not be started.");
		var stdout = process.StandardOutput.ReadToEnd().Trim();
		await process.WaitForExitAsync();
		Console.WriteLine(stdout);
	}

	/// <summary>
	/// Downloads the OpenJDK release corresponding to the specified Java version.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <param name="java">The major version of the Java development kit.</param>
	/// <returns>The path to the downloaded ZIP archive.</returns>
	private static async Task<string> DownloadArchive(HttpClient httpClient, int java) {
		var os = true switch {
			true when OperatingSystem.IsMacOS() => "macOS",
			true when OperatingSystem.IsWindows() => "windows",
			_ => "linux"
		};

		var path = Path.Join(Path.GetTempPath(), $"microsoft-jdk-{java}-{os}-x64.{(OperatingSystem.IsWindows() ? "zip" : "tar.gz")}");
		var file = Path.GetFileName(path);

		Console.WriteLine($"Downloading file \"{file}\"...");
		var bytes = await httpClient.GetByteArrayAsync($"https://aka.ms/download-jdk/{file}");
		File.WriteAllBytes(path, bytes);
		return path;
	}

	/// <summary>
	/// Extracts the TAR/ZIP archive located at the specified path.
	/// </summary>
	/// <param name="path">The path to the downloaded TAR/ZIP archive.</param>
	/// <param name="output">The path to the output directory.</param>
	private static void ExtractArchive(string path, DirectoryInfo output) {
		Console.WriteLine($"Extracting file \"{Path.GetFileName(path)}\" into directory \"{output.FullName}\"...");

		if (string.Equals(Path.GetExtension(path), ".zip", StringComparison.InvariantCultureIgnoreCase)) {
			using var zipArchive = ZipFile.OpenRead(path);
			zipArchive.ExtractToDirectory(output.FullName, overwriteFiles: true);
		}
		else {
			var args = new[] { $"--directory={output.FullName}", "--extract", $"--file={path}", "--strip-components=1" };
			using var process = Process.Start("tar", args) ?? throw new Exception(@"The ""tar"" process could not be started.");
			process.WaitForExit(); // TODO ExitCode != 0 throw Exception
		}
	}
}

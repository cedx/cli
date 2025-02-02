namespace Belin.Cli.CommandLine;

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
		this.SetHandler(Execute, outputOption, javaOption.FromAmong(["21", "17", "11", "8"]));
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="java">The major version of the Java development kit.</param>
	/// <returns>The exit code.</returns>
	private async Task<int> Execute(DirectoryInfo output, int java) {
		if (!this.CheckPrivilege(output)) return 1;

		using var httpClient = this.CreateHttpClient();
		this.ExtractZipFile(await DownloadArchive(httpClient, java), output); // TODO Strip the subfolder!!!
		Console.WriteLine(this.GetExecutableVersion(output, @"bin\java.exe"));
		return 0;
	}

	/// <summary>
	/// Downloads the OpenJDK release corresponding to the specified Java version.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <param name="java">The major version of the Java development kit.</param>
	/// <returns>The path to the downloaded ZIP archive.</returns>
	private static async Task<FileInfo> DownloadArchive(HttpClient httpClient, int java) {
		var path = Path.Join(Path.GetTempPath(), $"microsoft-jdk-{java}-windows-x64.zip");
		var file = Path.GetFileName(path);

		Console.WriteLine($"Downloading file \"{file}\"...");
		var bytes = await httpClient.GetByteArrayAsync($"https://aka.ms/download-jdk/{file}");
		File.WriteAllBytes(path, bytes);
		return new FileInfo(path);
	}
}

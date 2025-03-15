namespace Belin.Cli.Setup;

/// <summary>
/// Downloads and installs the latest OpenJDK release.
/// </summary>
public sealed class JdkCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public JdkCommand(): base("jdk", "Set up the latest OpenJDK release.") {
		var javaOption = new Option<int>(["-j", "--java"], () => 21, "The major version of the Java development kit.") { ArgumentHelpName = "version" };
		var outputOption = new OutputOption(new DirectoryInfo(OperatingSystem.IsWindows() ? @"C:\Program Files\OpenJDK" : "/opt/openjdk"));

		Add(javaOption.FromAmong(["21", "17", "11", "8"]));
		Add(outputOption);
		this.SetHandler(Execute, outputOption, javaOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="java">The major version of the Java development kit.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(DirectoryInfo output, int java) {
		if (!this.CheckPrivilege(output)) return 1;

		using var httpClient = this.CreateHttpClient();
		this.ExtractZipFile(await DownloadArchive(httpClient, java), output, strip: 1);
		Console.WriteLine(this.GetExecutableVersion(output, "bin/java"));
		return 0;
	}

	/// <summary>
	/// Downloads the OpenJDK release corresponding to the specified Java version.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	/// <param name="java">The major version of the Java development kit.</param>
	/// <returns>The path to the downloaded ZIP archive.</returns>
	private static async Task<FileInfo> DownloadArchive(HttpClient httpClient, int java) {
		var (operatingSystem, fileExtension) = true switch {
			true when OperatingSystem.IsMacOS() => ("macOS", "tar.gz"),
			true when OperatingSystem.IsWindows() => ("windows", "zip"),
			_ => ("linux", "tar.gz")
		};

		var file = $"microsoft-jdk-{java}-{operatingSystem}-x64.{fileExtension}";
		Console.WriteLine($"Downloading file \"{file}\"...");

		var bytes = await httpClient.GetByteArrayAsync($"https://aka.ms/download-jdk/{file}");
		var path = Path.Join(Path.GetTempPath(), file);
		File.WriteAllBytes(path, bytes);
		return new FileInfo(path);
	}
}

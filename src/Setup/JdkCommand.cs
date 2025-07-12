namespace Belin.Cli.Setup;

/// <summary>
/// Downloads and installs the latest OpenJDK release.
/// </summary>
public class JdkCommand: Command {

	/// <summary>
	/// The major version of the Java development kit.
	/// </summary>
	private readonly Option<int> javaOption = new Option<int>("--java", ["-j"]) {
		DefaultValueFactory = _ => 21,
		Description = "The major version of the Java development kit.",
		HelpName = "version"
	}.AcceptOnlyFromAmong(["8", "11", "17", "21"]);

	/// <summary>
	/// The path to the output directory.
	/// </summary>
	private readonly OutputOption outputOption = new(new DirectoryInfo(OperatingSystem.IsWindows() ? @"C:\Program Files\OpenJDK" : "/opt/openjdk"));

	/// <summary>
	/// The HTTP client.
	/// </summary>
	private readonly HttpClient httpClient;

	/// <summary>
	/// Creates a new <c>jdk</c> command.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	public JdkCommand(HttpClient httpClient): base("jdk", "Set up the latest OpenJDK release.") {
		this.httpClient = httpClient;
		Options.Add(javaOption);
		Options.Add(outputOption);
		SetAction(InvokeAsync);
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken) {
		var output = parseResult.GetValue(outputOption)!;
		if (!this.CheckPrivilege(output)) {
			Console.Error.WriteLine("You must run this command in an elevated prompt.");
			return 1;
		}

		var path = await DownloadArchive(parseResult.GetValue(javaOption));
		Console.WriteLine(@"Extracting file ""{0}"" into directory ""{1}""...", path.Name, output.Name);
		path.ExtractTo(output, strip: 1);
		Console.WriteLine("{0}", this.GetExecutableVersion(output, "bin/java"));
		return 0;
	}

	/// <summary>
	/// Downloads the OpenJDK release corresponding to the specified Java version.
	/// </summary>
	/// <param name="javaVersion">The major version of the Java development kit.</param>
	/// <returns>The path to the downloaded ZIP archive.</returns>
	private async Task<FileInfo> DownloadArchive(int javaVersion) {
		var (operatingSystem, fileExtension) = true switch {
			true when OperatingSystem.IsMacOS() => ("macOS", "tar.gz"),
			true when OperatingSystem.IsWindows() => ("windows", "zip"),
			_ => ("linux", "tar.gz")
		};

		var file = $"microsoft-jdk-{javaVersion}-{operatingSystem}-x64.{fileExtension}";
		Console.WriteLine(@"Downloading file ""{0}""...", file);

		var bytes = await httpClient.GetByteArrayAsync($"https://aka.ms/download-jdk/{file}");
		var path = Path.Join(Path.GetTempPath(), file);
		File.WriteAllBytes(path, bytes);
		return new FileInfo(path);
	}
}

namespace Belin.Cli.Setup;

using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;

/// <summary>
/// Downloads and installs the latest OpenJDK release.
/// </summary>
public class JdkCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public JdkCommand(): base("jdk", "Set up the latest OpenJDK release.") {
		var versions = new[] { "21", "17", "11", "8" };
		Add(new Option<int>(["-j", "--java"], () => 21, "The major version of the Java development kit.") { ArgumentHelpName = "version" }.FromAmong(versions));
		Add(new OutputOption(new DirectoryInfo(OperatingSystem.IsWindows() ? @"C:\Program Files\OpenJDK" : "/opt/openjdk")));
	}

	/// <summary>
	/// The command handler.
	/// </summary>
	/// <param name="logger">The logging service.</aparam>
	public class CommandHandler(ILogger<JdkCommand> logger): ICommandHandler {

		/// <summary>
		/// The major version of the Java development kit.
		/// </summary>
		public int Java { get; set; }

		/// <summary>
		/// The path to the output directory.
		/// </summary>
		public required DirectoryInfo Out { get; set; }

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public int Invoke(InvocationContext context) => InvokeAsync(context).Result;

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public async Task<int> InvokeAsync(InvocationContext context) {
			Console.WriteLine(Java);
			Console.WriteLine(Out);

			if (!this.CheckPrivilege(Out)) {
				logger.LogError("You must run this command in an elevated prompt.");
				return 1;
			}

			var path = await DownloadArchive(SetupCommand.CreateHttpClient());
			logger.LogInformation(@"Extracting file ""{Input}"" into directory ""{Output}""...", path.Name, Out.Name);
			path.ExtractTo(Out, strip: 1);
			logger.LogInformation("{Version}", SetupCommand.GetExecutableVersion(Out, "bin/java"));
			return 0;
		}

		/// <summary>
		/// Downloads the OpenJDK release corresponding to the specified Java version.
		/// </summary>
		/// <param name="httpClient">The HTTP client.</param>
		/// <returns>The path to the downloaded ZIP archive.</returns>
		private async Task<FileInfo> DownloadArchive(HttpClient httpClient) {
			var (operatingSystem, fileExtension) = true switch {
				true when OperatingSystem.IsMacOS() => ("macOS", "tar.gz"),
				true when OperatingSystem.IsWindows() => ("windows", "zip"),
				_ => ("linux", "tar.gz")
			};

			var file = $"microsoft-jdk-{Java}-{operatingSystem}-x64.{fileExtension}";
			logger.LogInformation(@"Downloading file ""{File}""...", file);

			var bytes = await httpClient.GetByteArrayAsync($"https://aka.ms/download-jdk/{file}");
			var path = Path.Join(Path.GetTempPath(), file);
			File.WriteAllBytes(path, bytes);
			return new FileInfo(path);
		}
	}
}

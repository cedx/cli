namespace Belin.Cli.Commands;

/// <summary>
/// Downloads and installs the latest OpenJDK release.
/// </summary>
public class JdkCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public JdkCommand(): base("jdk", "Download and install the latest OpenJDK release.") {
		var outputOption = new OutputOption(new DirectoryInfo(OperatingSystem.IsWindows() ? @"C:\Program Files\OpenJDK" : "/opt/openjdk"));
		Add(outputOption);
		this.SetHandler(Execute, outputOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	private async Task Execute(DirectoryInfo output) {
		using var httpClient = new HttpClient();
	}
}

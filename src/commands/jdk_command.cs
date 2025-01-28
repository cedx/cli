namespace Belin.Cli.Commands;

/// <summary>
/// Downloads and installs the latest OpenJDK release.
/// </summary>
public class JdkCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public JdkCommand(): base("jdk", "Download and install the latest OpenJDK release.") {
		Add(new OutputOption(OperatingSystem.IsWindows() ? @"C:\Program Files\OpenJDK" : "/opt/openjdk"));
		this.SetHandler(Execute);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	private void Execute() {

	}
}

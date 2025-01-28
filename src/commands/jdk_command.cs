namespace Belin.Cli.Commands;

/// <summary>
/// Downloads and installs the latest OpenJDK release.
/// </summary>
public class JdkCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public JdkCommand(): base("jdk", "Download and install the latest OpenJDK release.") {
		this.SetHandler(Run);
	}

	/// <summary>
	/// Runs this command.
	/// </summary>
	private void Run() {

	}
}

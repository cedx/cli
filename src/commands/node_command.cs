namespace Belin.Cli.Commands;

/// <summary>
/// Downloads and installs the latest Node.js release.
/// </summary>
public class NodeCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public NodeCommand(): base("node", "Download and install the latest Node.js release.") {
		this.SetHandler(Run);
	}

	/// <summary>
	/// Runs this command.
	/// </summary>
	private void Run() {

	}
}

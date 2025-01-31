namespace Belin.Cli.Commands;

/// <summary>
/// Downloads and installs the latest Node.js release.
/// </summary>
public class NodeCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public NodeCommand(): base("node", "Download and install the latest Node.js release.") {
		Add(new OutputOption(new DirectoryInfo(OperatingSystem.IsWindows() ? @"C:\Program Files\Node.js" : "/usr/local")));
		this.SetHandler(Execute);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	private void Execute() {

	}
}

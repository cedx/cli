namespace Belin.Cli.CommandLine;

using Belin.Cli.CommandLine.Setup;

/// <summary>
/// Downloads and installs a runtime environment.
/// </summary>
public class SetupCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public SetupCommand(): base("setup", "Download and install a runtime environment.") {
		Add(new JdkCommand());
		Add(new NodeCommand());
		Add(new PhpCommand());
	}
}

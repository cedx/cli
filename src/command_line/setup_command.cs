namespace Belin.Cli.CommandLine;

using Belin.Cli.CommandLine.Setup;

/// <summary>
/// TODO
/// </summary>
public class SetupCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public SetupCommand(): base("setup", "TODO") {
		Add(new JdkCommand());
		Add(new NodeCommand());
		Add(new PhpCommand());
	}
}

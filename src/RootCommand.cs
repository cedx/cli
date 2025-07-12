namespace Belin.Cli;

/// <summary>
/// The root command.
/// </summary>
public class RootCommand: System.CommandLine.RootCommand {

	/// <summary>
	/// Creates a new root command.
	/// </summary>
	/// <param name="iconvCommand">The `iconv` subcommand.</param>
	/// <param name="setupCommand">The `setup` subcommand.</param>
	public RootCommand(IconvCommand iconvCommand, SetupCommand setupCommand): base("Command line interface of Cédric Belin, full stack developer.") {
		Subcommands.Add(iconvCommand);
		// Add(new MySqlCommand());
		// Add(new NssmCommand());
		Subcommands.Add(setupCommand);
	}
}

namespace Belin.Cli;

/// <summary>
/// The root command.
/// </summary>
public class RootCommand: System.CommandLine.RootCommand {

	/// <summary>
	/// Creates a new root command.
	/// </summary>
	/// <param name="iconv">The <c>iconv</c> subcommand.</param>
	/// <param name="nssm">The <c>nssm</c> subcommand.</param>
	/// <param name="setup">The <c>setup</c> subcommand.</param>
	public RootCommand(IconvCommand iconv, NssmCommand nssm, SetupCommand setup): base("Command line interface of Cédric Belin, full stack developer.") {
		Subcommands.Add(iconv);
		// Add(new MySqlCommand());
		Subcommands.Add(nssm);
		Subcommands.Add(setup);
	}
}

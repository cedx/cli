namespace Belin.Cli.Commands;

/// <summary>
/// TODO
/// </summary>
public class MySqlCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public MySqlCommand(): base("mysql", "TODO") {
		AddAlias("mariadb");
		AddGlobalOption(new DsnOption());
		Add(new BackupCommand());
		Add(new CharsetCommand());
		Add(new EngineCommand());
		Add(new OptimizeCommand());
		Add(new RestoreCommand());
	}
}

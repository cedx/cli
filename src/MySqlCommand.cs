namespace Belin.Cli;

using Belin.Cli.MySql;

/// <summary>
/// Manages MariaDB/MySQL databases.
/// </summary>
public class MySqlCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public MySqlCommand(): base("mysql", "Manage MariaDB/MySQL databases.") {
		var dsnOption = new DsnOption();
		AddGlobalOption(dsnOption);
		Add(new BackupCommand(dsnOption));
		Add(new CharsetCommand(dsnOption));
		Add(new EngineCommand(dsnOption));
		Add(new OptimizeCommand(dsnOption));
		Add(new RestoreCommand(dsnOption));
	}
}

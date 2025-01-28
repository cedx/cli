namespace Belin.Cli.Commands.MySql;

/// <summary>
/// Backups a set of MariaDB/MySQL tables.
/// </summary>
public class BackupCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public BackupCommand(): base("db-backup", "Backup a set of MariaDB/MySQL tables.") {
		Add(new DsnOption());
		this.SetHandler(Run);
	}

	/// <summary>
	/// Runs this command.
	/// </summary>
	private void Run() {

	}
}

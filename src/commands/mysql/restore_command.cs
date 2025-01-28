namespace Belin.Cli.Commands.MySql;

/// <summary>
/// Restores a set of MariaDB/MySQL tables.
/// </summary>
public class RestoreCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public RestoreCommand(): base("db-restore", "Restore a set of MariaDB/MySQL tables.") {
		Add(new DsnOption());
		this.SetHandler(Execute);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	public void Execute() {

	}
}

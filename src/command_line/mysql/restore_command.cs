namespace Belin.Cli.CommandLine.MySql;

/// <summary>
/// Restores a set of MariaDB/MySQL tables.
/// </summary>
public class RestoreCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public RestoreCommand(DsnOption dsnOption): base("restore", "Restore a set of MariaDB/MySQL tables.") {
		this.SetHandler(Execute, dsnOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	private void Execute(Uri dsn) {

	}
}

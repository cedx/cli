namespace Belin.Cli.CommandLine.MySql;

/// <summary>
/// Backups a set of MariaDB/MySQL tables.
/// </summary>
public class BackupCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public BackupCommand(DsnOption dsnOption): base("backup", "Backup a set of MariaDB/MySQL tables.") {
		var schemaOption = new SchemaOption();
		Add(schemaOption);
		var tableOption = new TableOption();
		Add(tableOption);
		this.SetHandler(Execute, dsnOption, schemaOption, tableOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	private void Execute(Uri dsn, string? schema, string[] tables) {

	}
}

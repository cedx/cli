namespace Belin.Cli.CommandLine.MySql;

/// <summary>
/// Alters the storage engine of MariaDB/MySQL tables.
/// </summary>
public class EngineCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public EngineCommand(DsnOption dsnOption): base("db-engine", "Alter the storage engine of MariaDB/MySQL tables.") {
		var schemaOption = new SchemaOption();
		var tableOption = new TableOption();

		Add(schemaOption);
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

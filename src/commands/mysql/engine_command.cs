namespace Belin.Cli.Commands.MySql;

/// <summary>
/// Alters the storage engine of MariaDB/MySQL tables.
/// </summary>
public class EngineCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public EngineCommand(): base("db-engine", "Alter the storage engine of MariaDB/MySQL tables.") {
		var dsnOption = new DsnOption();
		var schemaOption = new SchemaOption();
		var tableOption = new TableOption();

		Add(dsnOption);
		Add(schemaOption);
		Add(tableOption);

		this.SetHandler(Execute, dsnOption, schemaOption, tableOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	public void Execute(Uri dsn, string? schema, string[] tables) {

	}
}

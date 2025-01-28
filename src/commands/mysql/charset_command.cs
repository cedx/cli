namespace Belin.Cli.Commands.MySql;

/// <summary>
/// Alters the character set of MariaDB/MySQL tables.
/// </summary>
public class CharsetCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public CharsetCommand(): base("db-charset", "Alter the character set of MariaDB/MySQL tables.") {
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

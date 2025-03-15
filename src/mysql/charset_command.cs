namespace Belin.Cli.MySql;

using MySqlConnector;

/// <summary>
/// Alters the character set of MariaDB/MySQL tables.
/// </summary>
public sealed class CharsetCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public CharsetCommand(DsnOption dsnOption): base("charset", "Alter the character set of MariaDB/MySQL tables.") {
		var collationArgument = new Argument<string>("collation", "The name of the new character set.");
		var schemaOption = new SchemaOption();
		var tableOption = new TableOption();

		Add(collationArgument);
		Add(schemaOption);
		Add(tableOption);
		this.SetHandler(Execute, dsnOption, collationArgument, schemaOption, tableOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <param name="collation">The name of the new character set.</param>
	/// <param name="schemaName">The schema name.</param>
	/// <param name="tableNames">The table names.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(Uri dsn, string collation, string? schemaName, string[] tableNames) {
		var noSchema = string.IsNullOrWhiteSpace(schemaName);
		if (tableNames.Length > 0 && noSchema) {
			Console.WriteLine($"The table \"{tableNames[0]}\" requires that a schema be specified.");
			return 1;
		}

		using var connection = this.CreateMySqlConnection(dsn);
		var schemas = noSchema ? connection.GetSchemas() : [new Schema { Name = schemaName! }];
		var tables = schemas.SelectMany(schema => tableNames.Length > 0
			? tableNames.Select(table => new Table { Name = table, Schema = schema.Name })
			: connection.GetTables(schema));

		using var disableForeignKeys = new MySqlCommand("SET foreign_key_checks = 0", connection);
		disableForeignKeys.ExecuteNonQuery();
		foreach (var table in tables.Where(item => !string.Equals(item.Collation, collation, StringComparison.InvariantCultureIgnoreCase)))
			AlterTable(connection, table, collation);

		using var enableForeignKeys = new MySqlCommand("SET foreign_key_checks = 1", connection);
		enableForeignKeys.ExecuteNonQuery();
		return await Task.FromResult(0);
	}

	/// <summary>
	/// Alters the specified database table.
	/// </summary>
	/// <param name="connection">The database connection.</param>
	/// <param name="table">The table to alter.</param>
	/// <param name="collation">The name of the new character set.</param>
	private static void AlterTable(MySqlConnection connection, Table table, string collation) {
		var qualifiedName = table.GetQualifiedName(escape: true);
		Console.WriteLine($"Processing: {qualifiedName}");
		var query = $"ALTER TABLE {qualifiedName} CONVERT TO CHARACTER SET {collation.Split('_')[0]} COLLATE {collation}";
		using var command = new MySqlCommand(query, connection);
		command.ExecuteNonQuery();
	}
}

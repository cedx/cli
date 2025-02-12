namespace Belin.Cli.CommandLine.MySql;

using Belin.Cli.Data;
using MySqlConnector;

/// <summary>
/// Alters the character set of MariaDB/MySQL tables.
/// </summary>
public class CharsetCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public CharsetCommand(DsnOption dsnOption): base("charset", "Alter the character set of MariaDB/MySQL tables.") {
		var collationParameter = new Argument<string>("collation", "The name of the new character set.");
		var schemaOption = new SchemaOption();
		var tableOption = new TableOption();

		Add(collationParameter);
		Add(schemaOption);
		Add(tableOption);
		this.SetHandler(Execute, dsnOption, collationParameter, schemaOption, tableOption);
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

		using var connection = await this.CreateDbConnection(dsn);
		var schemas = noSchema ? connection.GetSchemas() : [new Schema { Name = schemaName! }];
		var tables = schemas.SelectMany(schema => tableNames.Length > 0
			? tableNames.Select(table => new Table { Name = table, Schema = schema.Name })
			: connection.GetTables(schema));

		using var disableForeignKeys = new MySqlCommand("SET foreign_key_checks = 0", connection);
		await disableForeignKeys.ExecuteNonQueryAsync();
		foreach (var table in tables.Where(item => !string.Equals(item.Collation, collation, StringComparison.InvariantCultureIgnoreCase)))
			await AlterTable(connection, table, collation);

		using var enableForeignKeys = new MySqlCommand("SET foreign_key_checks = 1", connection);
		await enableForeignKeys.ExecuteNonQueryAsync();
		return 0;
	}

	/// <summary>
	/// Alters the specified database table.
	/// </summary>
	/// <param name="connection">The database connection.</param>
	/// <param name="table">The table to alter.</param>
	/// <param name="collation">The name of the new character set.</param>
	/// <returns>Completes when the table has been altered.</returns>
	private static async Task AlterTable(MySqlConnection connection, Table table, string collation) {
		var qualifiedName = table.GetQualifiedName(escape: true);
		Console.WriteLine($"Processing: {qualifiedName}");
		var query = $"ALTER TABLE {qualifiedName} CONVERT TO CHARACTER SET {collation.Split('_')[0]} COLLATE {collation}";
		using var command = new MySqlCommand(query, connection);
		await command.ExecuteNonQueryAsync();
	}
}

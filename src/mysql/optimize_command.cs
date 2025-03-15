namespace Belin.Cli.MySql;

using MySqlConnector;

/// <summary>
/// Optimizes a set of MariaDB/MySQL tables.
/// </summary>
public sealed class OptimizeCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public OptimizeCommand(DsnOption dsnOption): base("optimize", "Optimize a set of MariaDB/MySQL tables.") {
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
	/// <param name="schemaName">The schema name.</param>
	/// <param name="tableNames">The table names.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(Uri dsn, string? schemaName, string[] tableNames) {
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

		foreach (var table in tables) OptimizeTable(connection, table);
		return await Task.FromResult(0);
	}

	/// <summary>
	/// Optimizes the specified database table.
	/// </summary>
	/// <param name="connection">The database connection.</param>
	/// <param name="table">The table to optimize.</param>
	private static void OptimizeTable(MySqlConnection connection, Table table) {
		var qualifiedName = table.GetQualifiedName(escape: true);
		Console.WriteLine($"Optimizing: {qualifiedName}");
		using var command = new MySqlCommand($"OPTIMIZE TABLE {qualifiedName}", connection);
		command.ExecuteNonQuery();
	}
}

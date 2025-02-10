namespace Belin.Cli.CommandLine.MySql;

using Belin.Cli.Data;
using MySqlConnector;

/// <summary>
/// Alters the storage engine of MariaDB/MySQL tables.
/// </summary>
public class EngineCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public EngineCommand(DsnOption dsnOption): base("engine", "Alter the storage engine of MariaDB/MySQL tables.") {
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
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(Uri dsn, string? schemaName, string[] tableNames) {
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

		// foreach (var table in tables) await OptimizeTable(connection, table);
		return 0;
	}

	/// <summary>
	/// Alters the specified database table.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="table"></param>
	/// <returns></returns>
	private static async Task AlterTable(MySqlConnection connection, Table table) {
	}
}

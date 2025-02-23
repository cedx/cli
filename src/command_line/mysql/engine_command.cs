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
		var engineArgument = new Argument<string>("engine", "The name of the new storage engine.");
		var schemaOption = new SchemaOption();
		var tableOption = new TableOption();

		Add(engineArgument);
		Add(schemaOption);
		Add(tableOption);
		this.SetHandler(Execute, dsnOption, engineArgument, schemaOption, tableOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <param name="engine">The name of the new storage engine.</param>
	/// <param name="schemaName">The schema name.</param>
	/// <param name="tableNames">The table names.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(Uri dsn, string engine, string? schemaName, string[] tableNames) {
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
		foreach (var table in tables.Where(item => !string.Equals(item.Collation, engine, StringComparison.InvariantCultureIgnoreCase)))
			AlterTable(connection, table, engine);

		using var enableForeignKeys = new MySqlCommand("SET foreign_key_checks = 1", connection);
		enableForeignKeys.ExecuteNonQuery();
		return await Task.FromResult(0);
	}

	/// <summary>
	/// Alters the specified database table.
	/// </summary>
	/// <param name="connection">The database connection.</param>
	/// <param name="table">The table to alter.</param>
	/// <param name="engine">The name of the new storage engine.</param>
	/// <returns>Completes when the table has been altered.</returns>
	private static void AlterTable(MySqlConnection connection, Table table, string engine) {
		var qualifiedName = table.GetQualifiedName(escape: true);
		Console.WriteLine($"Processing: {qualifiedName}");
		using var command = new MySqlCommand($"ALTER TABLE {qualifiedName} ENGINE = {engine}", connection);
		command.ExecuteNonQuery();
	}
}

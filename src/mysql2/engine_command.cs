namespace Belin.Cli.MySql;

using Dapper;
using System.Data;

/// <summary>
/// Alters the storage engine of MariaDB/MySQL tables.
/// </summary>
public sealed class EngineCommand: Command {

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
		this.SetHandler(Invoke, dsnOption, engineArgument, schemaOption, tableOption);
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <param name="engine">The name of the new storage engine.</param>
	/// <param name="schemaName">The schema name.</param>
	/// <param name="tableNames">The table names.</param>
	/// <returns>The exit code.</returns>
	public Task<int> Invoke(Uri dsn, string engine, string? schemaName, string[] tableNames) {
		var noSchema = string.IsNullOrWhiteSpace(schemaName);
		if (tableNames.Length > 0 && noSchema) {
			Console.WriteLine($"The table \"{tableNames[0]}\" requires that a schema be specified.");
			return Task.FromResult(1);
		}

		using var connection = new InformationSchema().OpenConnection(dsn);
		var schemas = noSchema ? connection.GetSchemas() : [new Schema { Name = schemaName! }];
		var tables = schemas.SelectMany(schema => tableNames.Length > 0
			? tableNames.Select(table => new Table { Name = table, Schema = schema.Name })
			: connection.GetTables(schema));

		connection.Execute("SET foreign_key_checks = 0");
		foreach (var table in tables.Where(item => !item.Engine.Equals(engine, StringComparison.OrdinalIgnoreCase)))
			AlterTable(connection, table, engine);

		connection.Execute("SET foreign_key_checks = 1");
		return Task.FromResult(0);
	}

	/// <summary>
	/// Alters the specified database table.
	/// </summary>
	/// <param name="connection">The database connection.</param>
	/// <param name="table">The table to alter.</param>
	/// <param name="engine">The name of the new storage engine.</param>
	private static void AlterTable(IDbConnection connection, Table table, string engine) {
		var qualifiedName = table.GetQualifiedName(escape: true);
		Console.WriteLine($"Processing: {qualifiedName}");
		connection.Execute($"ALTER TABLE {qualifiedName} ENGINE = {engine}");
	}
}

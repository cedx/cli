namespace Belin.Cli.MySql;

using System.Data;

/// <summary>
/// Alters the storage engine of MariaDB/MySQL tableNames.
/// </summary>
public class EngineCommand: Command {

	/// <summary>
	/// The name of the new storage engine.
	/// </summary>
	private readonly Argument<string> engineArgument = new("engine") {
		Description = "The name of the new storage engine."
	};

	/// <summary>
	/// The schema name.
	/// </summary>
	private readonly SchemaOption schemaOption = new();

	/// <summary>
	/// The table names (requires a schema).
	/// </summary>
	private readonly TableOption tableOption = new();

	/// <summary>
	/// The database context.
	/// </summary>
	private readonly InformationSchema informationSchema;

	/// <summary>
	/// Creates a new <c>engine</c> command.
	/// </summary>
	/// <param name="informationSchema">The database context.</param>
	public EngineCommand(InformationSchema informationSchema): base("engine", "Alter the storage engine of MariaDB/MySQL tableNames.") {
		this.informationSchema = informationSchema;
		Arguments.Add(engineArgument);
		Options.Add(schemaOption);
		Options.Add(tableOption);
		SetAction(InvokeAsync);
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <returns>The exit code.</returns>
	public int Invoke(ParseResult parseResult) {
		var schemaName = parseResult.GetValue(schemaOption);
		var tableNames = parseResult.GetValue(tableOption)!;

		var noSchema = string.IsNullOrWhiteSpace(schemaName);
		if (tableNames.Length > 0 && noSchema) {
			Console.Error.WriteLine(@"The table ""{0}"" requires that a schema be specified.", tableNames[0]);
			return 1;
		}

		using var connection = informationSchema.CreateConnection(new Uri(parseResult.GetRequiredValue(MySqlCommand.dsnOption)));
		var schemas = noSchema ? connection.GetSchemas() : [new Schema { Name = schemaName! }];
		var tables = schemas.SelectMany(schema => tableNames.Length > 0
			? tableNames.Select(table => new Table { Name = table, Schema = schema.Name })
			: connection.GetTables(schema));

		var engine = parseResult.GetRequiredValue(engineArgument);
		connection.Execute("SET foreign_key_checks = 0");
		foreach (var table in tables.Where(item => !item.Engine.Equals(engine, StringComparison.OrdinalIgnoreCase))) AlterTable(connection, table, engine);
		connection.Execute("SET foreign_key_checks = 1");
		return 0;
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The exit code.</returns>
	public Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken) => Task.FromResult(Invoke(parseResult));

	/// <summary>
	/// Alters the specified database table.
	/// </summary>
	/// <param name="connection">The database connection.</param>
	/// <param name="table">The table to alter.</param>
	/// <param name="engine">The name of the new storage engine.</param>
	private static void AlterTable(IDbConnection connection, Table table, string engine) {
		var qualifiedName = table.GetQualifiedName(escape: true);
		Console.WriteLine("Processing: {0}", qualifiedName);
		connection.Execute($"ALTER TABLE {qualifiedName} ENGINE = {engine}");
	}
}

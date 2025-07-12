namespace Belin.Cli.MySql;

using System.Data;

/// <summary>
/// Optimizes a set of MariaDB/MySQL tables.
/// </summary>
public class OptimizeCommand: Command {

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
	/// Creates a new <c>optimize</c> command.
	/// </summary>
	/// <param name="informationSchema">The database context.</param>
	public OptimizeCommand(InformationSchema informationSchema): base("optimize", "Optimize a set of MariaDB/MySQL tables.") {
		this.informationSchema = informationSchema;
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

		foreach (var table in tables) OptimizeTable(connection, table);
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
	/// Optimizes the specified database table.
	/// </summary>
	/// <param name="connection">The database connection.</param>
	/// <param name="table">The table to optimize.</param>
	private static void OptimizeTable(IDbConnection connection, Table table) {
		var qualifiedName = table.GetQualifiedName(escape: true);
		Console.WriteLine("Optimizing: {0}", qualifiedName);
		connection.Execute($"OPTIMIZE TABLE {qualifiedName}");
	}
}

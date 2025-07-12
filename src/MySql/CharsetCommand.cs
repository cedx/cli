namespace Belin.Cli.MySql;

using System.Data;

/// <summary>
/// Alters the character set of MariaDB/MySQL tables.
/// </summary>
public class CharsetCommand: Command {

	/// <summary>
	/// The name of the new character set.
	/// </summary>
	private readonly Argument<string> collationArgument = new("collation") {
		Description = "The name of the new character set."
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
	/// Creates a new <c>charset</c> command.
	/// </summary>
	/// <param name="informationSchema">The database context.</param>
	public CharsetCommand(InformationSchema informationSchema): base("charset", "Alter the character set of MariaDB/MySQL tables.") {
		this.informationSchema = informationSchema;
		Arguments.Add(collationArgument);
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
		var tableNames = parseResult.GetRequiredValue(tableOption);

		var noSchema = string.IsNullOrWhiteSpace(schemaName);
		if (tableNames.Length > 0 && noSchema) {
			Console.Error.WriteLine(@"The table ""{0}"" requires that a schema be specified.", tableNames[0]);
			return 1;
		}

		using var connection = informationSchema.CreateConnection(parseResult.GetRequiredValue(MySqlCommand.dsnOption));
		var schemas = noSchema ? connection.GetSchemas() : [new Schema { Name = schemaName! }];
		var tables = schemas.SelectMany(schema => tableNames.Length > 0
			? tableNames.Select(table => new Table { Name = table, Schema = schema.Name })
			: connection.GetTables(schema));

		var collation = parseResult.GetRequiredValue(collationArgument);
		connection.Execute("SET foreign_key_checks = 0");
		foreach (var table in tables.Where(item => !item.Collation.Equals(collation, StringComparison.OrdinalIgnoreCase))) AlterTable(connection, table, collation);
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
	/// <param name="collation">The name of the new character set.</param>
	private static void AlterTable(IDbConnection connection, Table table, string collation) {
		var qualifiedName = table.GetQualifiedName(escape: true);
		Console.WriteLine("Processing: {0}", qualifiedName);
		connection.Execute($"ALTER TABLE {qualifiedName} CONVERT TO CHARACTER SET {collation.Split('_')[0]} COLLATE {collation}");
	}
}

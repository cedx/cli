namespace Belin.Cli.MySql;

using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.Data;

/// <summary>
/// Optimizes a set of MariaDB/MySQL tables.
/// </summary>
public class OptimizeCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public OptimizeCommand(): base("optimize", "Optimize a set of MariaDB/MySQL tables.") {
		Add(new SchemaOption());
		Add(new TableOption());
	}

	/// <summary>
	/// The command handler.
	/// </summary>
	/// <param name="db">The dabase context.</param>
	/// <param name="logger">The logging service.</aparam>
	public class CommandHandler(InformationSchema db, ILogger<OptimizeCommand> logger): ICommandHandler {

		/// <summary>
		/// The connection string.
		/// </summary>
		public required Uri Dsn { get; set; }

		/// <summary>
		/// The schema name.
		/// </summary>
		public string? Schema { get; set; }

		/// <summary>
		/// The table names.
		/// </summary>
		public string[] Table { get; set; } = [];

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public int Invoke(InvocationContext context) {
			var noSchema = string.IsNullOrWhiteSpace(Schema);
			if (Table.Length > 0 && noSchema) {
				logger.LogError(@"The table ""{Table}"" requires that a schema be specified.", Table[0]);
				return 1;
			}

			using var connection = db.CreateConnection(Dsn);
			var schemas = noSchema ? connection.GetSchemas() : [new Schema { Name = Schema! }];
			var tables = schemas.SelectMany(schema => Table.Length > 0
				? Table.Select(table => new Table { Name = table, Schema = schema.Name })
				: connection.GetTables(schema));

			foreach (var table in tables) OptimizeTable(connection, table);
			return 0;
		}

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public Task<int> InvokeAsync(InvocationContext context) => Task.FromResult(Invoke(context));

		/// <summary>
		/// Optimizes the specified database table.
		/// </summary>
		/// <param name="connection">The database connection.</param>
		/// <param name="table">The table to optimize.</param>
		private void OptimizeTable(IDbConnection connection, Table table) {
			var qualifiedName = table.GetQualifiedName(escape: true);
			logger.LogInformation("Optimizing: {QualifiedName}", qualifiedName);
			connection.Execute($"OPTIMIZE TABLE {qualifiedName}");
		}
	}
}

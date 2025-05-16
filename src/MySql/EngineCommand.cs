namespace Belin.Cli.MySql;

using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.Data;

/// <summary>
/// Alters the storage engine of MariaDB/MySQL tables.
/// </summary>
public class EngineCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public EngineCommand(): base("engine", "Alter the storage engine of MariaDB/MySQL tables.") {
		Add(new Argument<string>("engine", "The name of the new storage engine."));
		Add(new SchemaOption());
		Add(new TableOption());
	}

	/// <summary>
	/// The command handler.
	/// </summary>
	/// <param name="db">The dabase context.</param>
	/// <param name="logger">The logging service.</aparam>
	public class CommandHandler(InformationSchema db, ILogger<BackupCommand> logger): ICommandHandler {

		/// <summary>
		/// The connection string.
		/// </summary>
		public required Uri Dsn { get; set; }

		/// <summary>
		/// The name of the new storage engine.
		/// </summary>
		public required string Engine { get; set; }

		/// <summary>
		/// The schema name.
		/// </summary>
		public string Schema { get; set; } = string.Empty;

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
			var schemas = noSchema ? connection.GetSchemas() : [new Schema { Name = Schema }];
			var tables = schemas.SelectMany(schema => Table.Length > 0
				? Table.Select(table => new Table { Name = table, Schema = schema.Name })
				: connection.GetTables(schema));

			connection.Execute("SET foreign_key_checks = 0");
			foreach (var table in tables.Where(item => !item.Engine.Equals(Engine, StringComparison.OrdinalIgnoreCase))) AlterTable(connection, table);
			connection.Execute("SET foreign_key_checks = 1");
			return 0;
		}

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public Task<int> InvokeAsync(InvocationContext context) => Task.FromResult(Invoke(context));

		/// <summary>
		/// Alters the specified database table.
		/// </summary>
		/// <param name="connection">The database connection.</param>
		/// <param name="table">The table to alter.</param>
		private void AlterTable(IDbConnection connection, Table table) {
			var qualifiedName = table.GetQualifiedName(escape: true);
			logger.LogInformation("Processing: {QualifiedName}", qualifiedName);
			connection.Execute($"ALTER TABLE {qualifiedName} ENGINE = {Engine}");
		}
	}
}

namespace Belin.Cli.MySql;

using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.Data;
using System.Diagnostics;
using System.Text.Json;

/// <summary>
/// Backups a set of MariaDB/MySQL tables.
/// </summary>
public class BackupCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public BackupCommand(): base("backup", "Backup a set of MariaDB/MySQL tables.") {
		var backupFormats = new[] { BackupFormat.JsonLines, BackupFormat.SqlDump };
		Add(new Argument<DirectoryInfo>("directory", "The path to the output directory."));
		Add(new Option<string>(["-f", "--format"], () => BackupFormat.SqlDump, "The format of the output files.").FromAmong(backupFormats));
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
		/// The path to the output directory.
		/// </summary>
		public required DirectoryInfo Directory { get; set; }

		/// <summary>
		/// The connection string.
		/// </summary>
		public required Uri Dsn { get; set; }

		/// <summary>
		/// The format of the output files.
		/// </summary>
		public string Format { get; set; } = BackupFormat.SqlDump;

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

			try {
				if (Format == BackupFormat.JsonLines) logger.LogWarning(@"Warning: the ""JSON Lines"" format does not export INVISIBLE columns.");
				Directory.Create();

				using var connection = db.CreateConnection(Dsn);
				foreach (var schema in noSchema ? connection.GetSchemas() : [new Schema { Name = Schema }]) {
					var entity = Table.Length == 1 ? $"{schema.Name}.{Table[0]}" : schema.Name;
					logger.LogInformation("Exporting: {Entity}", entity);
					if (Format == BackupFormat.JsonLines) ExportToJsonLines(connection, schema);
					else ExportToSqlDump(schema);
				}

				return 0;
			}
			catch (Exception e) {
				logger.LogError("{Message}", e.Message);
				return 2;
			}
		}

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public Task<int> InvokeAsync(InvocationContext context) => Task.FromResult(Invoke(context));

		/// <summary>
		/// Exports the specified schema to a set of JSON Lines files in the specified directory.
		/// </summary>
		/// <param name="connection">The database connection.</param>
		/// <param name="schema">The schema to export.</param>
		private void ExportToJsonLines(IDbConnection connection, Schema schema) {
			var tables = Table.Length > 0 ? Table.Select(table => new Table { Name = table, Schema = schema.Name }) : connection.GetTables(schema);
			foreach (var table in tables) {
				using var file = File.CreateText(Path.Join(Directory.FullName, $"{table.QualifiedName}.{BackupFormat.JsonLines}"));
				using var reader = connection.ExecuteReader($"SELECT * FROM {table.GetQualifiedName(escape: true)}");
				while (reader.Read()) {
					var record = new Dictionary<string, object?>();
					for (var i = 0; i < reader.FieldCount; i++) record[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
					file.WriteLine(JsonSerializer.Serialize(record));
				}
			}
		}

		/// <summary>
		/// Exports the specified schema to a SQL dump in the specified directory.
		/// </summary>
		/// <param name="schema">The schema to export.</param>
		/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
		private void ExportToSqlDump(Schema schema) {
			var entity = Table.Length == 1 ? $"{schema.Name}.{Table[0]}" : schema.Name;
			var file = $"{entity}.{BackupFormat.SqlDump}";

			var userInfo = Dsn.UserInfo.Split(':').Select(Uri.UnescapeDataString);
			var args = new List<string> {
				$"--default-character-set={Dsn.ParseQueryString()["charset"] ?? "utf8mb4"}",
				$"--host={Dsn.Host}",
				$"--password={userInfo.Last()}",
				$"--port={(Dsn.IsDefaultPort ? 3306 : Dsn.Port)}",
				$"--result-file={Path.Join(Directory.FullName, file)}",
				$"--user={userInfo.First()}"
			};

			var hosts = new[] { "::1", "127.0.0.1", "localhost" };
			if (!hosts.Contains(Dsn.Host)) args.Add("--compress");
			args.Add(schema.Name);
			args.AddRange(Table);

			var startInfo = new ProcessStartInfo("mysqldump", args) { CreateNoWindow = true, RedirectStandardError = true };
			using var process = Process.Start(startInfo) ?? throw new ProcessException(startInfo.FileName);

			var stderr = process.StandardError.ReadToEnd().Trim();
			process.WaitForExit();
			if (process.ExitCode != 0) throw new ProcessException(startInfo.FileName, stderr);
		}
	}
}

/// <summary>
/// Defines the format of the output files.
/// </summary>
public static class BackupFormat {

	/// <summary>
	/// The JSON Lines format.
	/// </summary>
	public const string JsonLines = "jsonl";

	/// <summary>
	/// The SQL format.
	/// </summary>
	public const string SqlDump = "sql";
}

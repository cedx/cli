namespace Belin.Cli.MySql;

using System.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Web;

/// <summary>
/// Backups a set of MariaDB/MySQL tables.
/// </summary>
public class BackupCommand: Command {

	/// <summary>
	/// The path to the output directory.
	/// </summary>
	private readonly Argument<DirectoryInfo> directoryArgument = new Argument<DirectoryInfo>("directory") {
		Description = "The path to the output directory."
	}.AcceptLegalFilePathsOnly();

	/// <summary>
	/// The format of the output files.
	/// </summary>
	private readonly Option<string> formatOption = new Option<string>("--format", ["-f"]) {
		DefaultValueFactory = _ => BackupFormat.SqlDump,
		Description = "The format of the output files."
	}.AcceptOnlyFromAmong([BackupFormat.JsonLines, BackupFormat.SqlDump]);

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
	/// Creates a new <c>backup</c> command.
	/// </summary>
	/// <param name="informationSchema">The database context.</param>
	public BackupCommand(InformationSchema informationSchema): base("backup", "Backup a set of MariaDB/MySQL tables.") {
		this.informationSchema = informationSchema;
		Arguments.Add(directoryArgument);
		Options.Add(formatOption);
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

		try {
			var format = parseResult.GetRequiredValue(formatOption);
			if (format == BackupFormat.JsonLines) Console.WriteLine(@"Warning: the ""JSON Lines"" format does not export INVISIBLE columns.");

			var directory = parseResult.GetRequiredValue(directoryArgument);
			directory.Create();

			var dsn = new Uri(parseResult.GetRequiredValue(MySqlCommand.dsnOption));
			using var connection = informationSchema.CreateConnection(dsn);
			foreach (var schema in noSchema ? connection.GetSchemas() : [new Schema { Name = schemaName! }]) {
				var entity = tableNames.Length == 1 ? $"{schema.Name}.{tableNames[0]}" : schema.Name;
				Console.WriteLine("Exporting: {0}", entity);
				if (format == BackupFormat.JsonLines) ExportToJsonLines(directory, connection, schema, tableNames);
				else ExportToSqlDump(directory, dsn, schema, tableNames);
			}

			return 0;
		}
		catch (Exception e) {
			Console.Error.WriteLine("{0}", e.Message);
			return 2;
		}
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The exit code.</returns>
	public Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken) => Task.FromResult(Invoke(parseResult));

	/// <summary>
	/// Exports the specified schema to a set of JSON Lines files in the specified directory.
	/// </summary>
	/// <param name="directory">The output directory.</param>
	/// <param name="connection">The database connection.</param>
	/// <param name="schema">The schema to export.</param>
	/// <param name="tableNames">The names of the tables to export.</param>
	private static void ExportToJsonLines(DirectoryInfo directory, IDbConnection connection, Schema schema, string[] tableNames) {
		var tables = tableNames.Length > 0 ? tableNames.Select(table => new Table { Name = table, Schema = schema.Name }) : connection.GetTables(schema);
		foreach (var table in tables) {
			using var file = File.CreateText(Path.Join(directory.FullName, $"{table.QualifiedName}.{BackupFormat.JsonLines}"));
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
	/// <param name="directory">The output directory.</param>
	/// <param name="dsn">The connection string.</param>
	/// <param name="schema">The schema to export.</param>
	/// <param name="tableNames">The names of the tables to export.</param>
	/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
	private static void ExportToSqlDump(DirectoryInfo directory, Uri dsn, Schema schema, string[] tableNames) {
		var entity = tableNames.Length == 1 ? $"{schema.Name}.{tableNames[0]}" : schema.Name;
		var file = $"{entity}.{BackupFormat.SqlDump}";

		var userInfo = dsn.UserInfo.Split(':').Select(Uri.UnescapeDataString);
		var args = new List<string> {
			$"--default-character-set={HttpUtility.ParseQueryString(dsn.Query)["charset"] ?? "utf8mb4"}",
			$"--host={dsn.Host}",
			$"--password={userInfo.Last()}",
			$"--port={(dsn.IsDefaultPort ? 3306 : dsn.Port)}",
			$"--result-file={Path.Join(directory.FullName, file)}",
			$"--user={userInfo.First()}"
		};

		var hosts = new[] { "::1", "127.0.0.1", "localhost" };
		if (!hosts.Contains(dsn.Host)) args.Add("--compress");
		args.Add(schema.Name);
		args.AddRange(tableNames);

		var startInfo = new ProcessStartInfo("mysqldump", args) { CreateNoWindow = true, RedirectStandardError = true };
		using var process = Process.Start(startInfo) ?? throw new ProcessException(startInfo.FileName);

		var stderr = process.StandardError.ReadToEnd().Trim();
		process.WaitForExit();
		if (process.ExitCode != 0) throw new ProcessException(startInfo.FileName, stderr);
	}
}

/// <summary>
/// Defines the format of the output files.
/// </summary>
internal static class BackupFormat {

	/// <summary>
	/// The JSON Lines format.
	/// </summary>
	public const string JsonLines = "jsonl";

	/// <summary>
	/// The SQL format.
	/// </summary>
	public const string SqlDump = "sql";
}

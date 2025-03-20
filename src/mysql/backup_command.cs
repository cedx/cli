namespace Belin.Cli.MySql;

using MySqlConnector;
using System.Diagnostics;
using System.Text.Json;

/// <summary>
/// Backups a set of MariaDB/MySQL tables.
/// </summary>
public sealed class BackupCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public BackupCommand(DsnOption dsnOption): base("backup", "Backup a set of MariaDB/MySQL tables.") {
		var directoryArgument = new Argument<DirectoryInfo>("directory", "The path to the output directory.");
		var formatOption = new Option<string>(["-f", "--format"], () => BackupFormat.SqlDump, "The format of the output files.");
		var schemaOption = new SchemaOption();
		var tableOption = new TableOption();

		Add(directoryArgument);
		Add(formatOption.FromAmong(BackupFormat.JsonLines, BackupFormat.SqlDump));
		Add(schemaOption);
		Add(tableOption);
		this.SetHandler(Execute, dsnOption, directoryArgument, formatOption, schemaOption, tableOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <param name="directory">The path to the output directory.</param>
	/// <param name="format">The format of the output files.</param>
	/// <param name="schemaName">The schema name.</param>
	/// <param name="tableNames">The table names.</param>
	/// <returns>The exit code.</returns>
	public Task<int> Execute(Uri dsn, DirectoryInfo directory, string format, string? schemaName, string[] tableNames) {
		var noSchema = string.IsNullOrWhiteSpace(schemaName);
		if (tableNames.Length > 0 && noSchema) {
			Console.WriteLine($"The table \"{tableNames[0]}\" requires that a schema be specified.");
			return Task.FromResult(1);
		}

		try {
			if (format == BackupFormat.JsonLines) Console.WriteLine(@"Warning: the ""JSON Lines"" format does not export INVISIBLE columns.");
			directory.Create();

			using var connection = this.CreateMySqlConnection(dsn);
			foreach (var schema in noSchema ? connection.GetSchemas() : [new Schema { Name = schemaName! }]) {
				var entity = tableNames.Length == 1 ? $"{schema.Name}.{tableNames[0]}" : schema.Name;
				Console.WriteLine($"Exporting: {entity}");
				if (format == BackupFormat.JsonLines) ExportToJsonLines(connection, schema, tableNames, directory);
				else ExportToSqlDump(dsn, schema, tableNames, directory);
			}

			return Task.FromResult(0);
		}
		catch (Exception e) {
			Console.WriteLine(e.Message);
			return Task.FromResult(2);
		}
	}

	/// <summary>
	/// Exports a data source to a set of JSON Lines files in the specified directory.
	/// </summary>
	/// <param name="connection">The database connection.</param>
	/// <param name="schema">The schema to export.</param>
	/// <param name="tableNames">The tables to export.</param>
	/// <param name="directory">The path to the output directory.</param>
	private static void ExportToJsonLines(MySqlConnection connection, Schema schema, string[] tableNames, DirectoryInfo directory) {
		var tables = tableNames.Length > 0 ? tableNames.Select(table => new Table { Name = table, Schema = schema.Name }) : connection.GetTables(schema);
		foreach (var table in tables) {
			using var command = connection.CreateCommand();
			command.CommandText = $"SELECT * FROM {table.GetQualifiedName(escape: true)}";

			using var file = File.CreateText(Path.Join(directory.FullName, $"{table.QualifiedName}.{BackupFormat.JsonLines}"));
			using var reader = command.ExecuteReader();
			while (reader.Read()) {
				var record = new Dictionary<string, object?>();
				for (var i = 0; i < reader.FieldCount; i++) record[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
				file.WriteLine(JsonSerializer.Serialize(record));
			}
		}
	}

	/// <summary>
	/// Exports a data source to a SQL dump in the specified directory.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <param name="schema">The schema to export.</param>
	/// <param name="tableNames">The tables to export.</param>
	/// <param name="directory">The path to the output directory.</param>
	/// <exception cref="ProcessException">An error occurred when starting the underlying process.</exception>
	private void ExportToSqlDump(Uri dsn, Schema schema, string[] tableNames, DirectoryInfo directory) {
		var entity = tableNames.Length == 1 ? $"{schema.Name}.{tableNames[0]}" : schema.Name;
		var file = $"{entity}.{BackupFormat.SqlDump}";

		var query = this.ParseQueryString(dsn.Query);
		var userInfo = dsn.UserInfo.Split(':').Select(Uri.UnescapeDataString);
		var args = new List<string> {
			$"--default-character-set={query["charset"] ?? "utf8mb4"}",
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

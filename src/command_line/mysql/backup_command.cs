namespace Belin.Cli.CommandLine.MySql;

using Belin.Cli.Data;
using MySqlConnector;
using System.Diagnostics;

/// <summary>
/// Backups a set of MariaDB/MySQL tables.
/// </summary>
public class BackupCommand: Command {

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
	public async Task<int> Execute(Uri dsn, DirectoryInfo directory, string format, string? schemaName, string[] tableNames) {
		var noSchema = string.IsNullOrWhiteSpace(schemaName);
		if (tableNames.Length > 0 && noSchema) {
			Console.WriteLine($"The table \"{tableNames[0]}\" requires that a schema be specified.");
			return 1;
		}

		if (format == BackupFormat.JsonLines) Console.WriteLine(@"Caution: the ""JSON Lines"" format does not export INVISIBLE columns.");
		using var connection = await this.CreateMySqlConnection(dsn);
		var schemas = noSchema ? connection.GetSchemas() : [new Schema { Name = schemaName! }];
		var tables = schemas.SelectMany(schema => tableNames.Length > 0
			? tableNames.Select(table => new Table { Name = table, Schema = schema.Name })
			: connection.GetTables(schema));

		directory.Create();
		foreach (var schema in schemas) {
			if (format == BackupFormat.JsonLines) await ExportToJsonLines(connection, schema, tableNames, directory);
			else await ExportToSqlDump(dsn, schema, tableNames, directory);
		}

		return 0;
	}

	/// <summary>
	/// Exports a data source to a set of JSON Lines files in the specified directory.
	/// </summary>
	/// <param name="connection">The database connection.</param>
	/// <param name="schema">The schema to export.</param>
	/// <param name="tableNames">The tables to export.</param>
	/// <param name="directory">The path to the output directory.</param>
	/// <returns>Completes when the specified schema has been exported.</returns>
	private static async Task ExportToJsonLines(MySqlConnection connection, Schema schema, string[] tableNames, DirectoryInfo directory) {
		var entity = tableNames.Length == 1 ? $"{schema.Name}.{tableNames[0]}" : schema.Name;
		Console.WriteLine($"Exporting: {entity}");

		var tables = tableNames.Length > 0 ? tableNames.Select(table => new Table { Name = table, Schema = schema.Name }) : connection.GetTables(schema);
		foreach (var table in tables) {
			await Task.FromResult("TODO");
		}
	}

	/// <summary>
	/// Exports a data source to a SQL dump in the specified directory.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <param name="schema">The schema to export.</param>
	/// <param name="tableNames">The tables to export.</param>
	/// <param name="directory">The path to the output directory.</param>
	/// <returns>Completes when the specified schema has been exported.</returns>
	private async Task ExportToSqlDump(Uri dsn, Schema schema, string[] tableNames, DirectoryInfo directory) {
		var entity = tableNames.Length == 1 ? $"{schema.Name}.{tableNames[0]}" : schema.Name;
		Console.WriteLine($"Exporting: {entity}");

		var file = $"{entity}.{BackupFormat.SqlDump}";
		var query = this.ParseQueryString(dsn.Query);
		var userInfo = dsn.UserInfo.Split(':').Select(Uri.UnescapeDataString).ToArray();

		var args = new List<string> {
			$"--default-character-set={query["charset"] ?? "utf8mb4"}",
			$"--host={dsn.Host}",
			$"--password={userInfo[1]}",
			$"--port={(dsn.IsDefaultPort ? 3306 : dsn.Port)}",
			$"--result-file={Path.Join(directory.FullName, file)}",
			$"--user={userInfo[0]}"
		};

		var hosts = new[] { "::1", "127.0.0.1", "localhost" };
		if (hosts.Contains(dsn.Host)) args.Add("--compress");
		args.Add(schema.Name);
		args.AddRange(tableNames);

		var startInfo = new ProcessStartInfo("mysqldump", args) { CreateNoWindow = true, RedirectStandardError = true };
		using var process = Process.Start(startInfo) ?? throw new Exception(@"The ""mysqldump"" process could not be started.");

		var stderr = process.StandardError.ReadToEnd().Trim();
		await process.WaitForExitAsync();
		if (process.ExitCode != 0) throw new Exception(stderr);
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

namespace Belin.Cli.CommandLine.MySql;

/// <summary>
/// Backups a set of MariaDB/MySQL tables.
/// </summary>
public class BackupCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public BackupCommand(DsnOption dsnOption): base("backup", "Backup a set of MariaDB/MySQL tables.") {
		var schemaOption = new SchemaOption();
		var tableOption = new TableOption();

		Add(schemaOption);
		Add(tableOption);
		this.SetHandler(Execute, dsnOption, schemaOption, tableOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(Uri dsn, string? schema, string[] tables) {
		if (tables.Length > 0 && schema is null) {
			Console.WriteLine($"The table \"{tables[0]}\" requires that a schema be specified.");
			return 1;
		}

		return await Task.FromResult(0);
	}
}

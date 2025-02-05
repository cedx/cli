namespace Belin.Cli.CommandLine.MySql;

/// <summary>
/// Optimizes a set of MariaDB/MySQL tables.
/// </summary>
public class OptimizeCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public OptimizeCommand(DsnOption dsnOption): base("optimize", "Optimize a set of MariaDB/MySQL tables.") {
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
		if (tables.Length > 0 && string.IsNullOrWhiteSpace(schema)) {
			Console.WriteLine($"The table \"{tables[0]}\" requires that a schema be specified.");
			return 1;
		}

		//Console.WriteLine(dsn.Scheme);
		return await Task.FromResult(0);
	}
}

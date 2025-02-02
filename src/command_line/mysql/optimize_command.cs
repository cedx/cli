namespace Belin.Cli.CommandLine.MySql;

/// <summary>
/// Optimizes a set of MariaDB/MySQL tables.
/// </summary>
public class OptimizeCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public OptimizeCommand(DsnOption dsnOption): base("optimize", "Optimize a set of MariaDB/MySQL tables.") {
		this.SetHandler(Execute, dsnOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute(Uri dsn) {
		return await Task.FromResult(0);
	}
}

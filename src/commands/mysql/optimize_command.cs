namespace Belin.Cli.Commands.MySql;

/// <summary>
/// Optimizes a set of MariaDB/MySQL tables.
/// </summary>
public class OptimizeCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public OptimizeCommand(DsnOption dsnOption): base("db-optimize", "Optimize a set of MariaDB/MySQL tables.") {
		this.SetHandler(Execute, dsnOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="dsn">The connection string.</param>
	private void Execute(Uri dsn) {

	}
}

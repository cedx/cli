namespace Belin.Cli.Commands.MySql;

/// <summary>
/// Alters the storage engine of MariaDB/MySQL tables.
/// </summary>
public class EngineCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public EngineCommand(): base("db-engine", "Alter the storage engine of MariaDB/MySQL tables.") {
		Add(new DsnOption());
		this.SetHandler(Run);
	}

	/// <summary>
	/// Runs this command.
	/// </summary>
	private void Run() {

	}
}

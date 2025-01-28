namespace Belin.Cli.Commands.MySql;

/// <summary>
/// Alters the character set of MariaDB/MySQL tables.
/// </summary>
public class CharsetCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public CharsetCommand(): base("db-charset", "Alter the character set of MariaDB/MySQL tables.") {
		Add(new DsnOption());
		this.SetHandler(Run);
	}

	/// <summary>
	/// Runs this command.
	/// </summary>
	private void Run() {

	}
}

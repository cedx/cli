namespace Belin.Cli.CommandLine;

using Belin.Cli.CommandLine.MySql;

/// <summary>
/// TODO
/// </summary>
public class MySqlCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public MySqlCommand(): base("mysql", "TODO") {
		var dsnOption = new DsnOption();
		AddGlobalOption(dsnOption);
		Add(new BackupCommand(dsnOption));
		Add(new CharsetCommand(dsnOption));
		Add(new EngineCommand(dsnOption));
		Add(new OptimizeCommand(dsnOption));
		Add(new RestoreCommand(dsnOption));
	}
}

namespace Belin.Cli.MySql;

/// <summary>
/// Provides the name of a database table.
/// </summary>
public class TableOption: Option<string[]> {

	/// <summary>
	/// Creates a new option.
	/// </summary>
	public TableOption(): base("--table", ["-t"]) {
		Description = "The table names (requires a schema).";
		HelpName = "name";
	}
}

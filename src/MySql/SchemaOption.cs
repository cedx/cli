namespace Belin.Cli.MySql;

/// <summary>
/// Provides the name of a database schema.
/// </summary>
public class SchemaOption: Option<string> {

	/// <summary>
	/// Creates a new option.
	/// </summary>
	public SchemaOption(): base("--schema", ["-s"]) {
		Description = "The schema name.";
		HelpName = "name";
	}
}

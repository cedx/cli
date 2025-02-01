namespace Belin.Cli.CommandLine.MySql;

/// <summary>
/// Provides the connection string of a data source.
/// </summary>
public class DsnOption: Option<Uri> {

	/// <summary>
	/// Creates a new option.
	/// </summary>
	public DsnOption(): base(["-d", "--dsn"], "The connection string.") {
		ArgumentHelpName = "uri";
		IsRequired = true;
	}
}

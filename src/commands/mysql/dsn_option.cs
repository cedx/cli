namespace Belin.Cli.Commands.MySql;

/// <summary>
/// Provides the connection string of a data source.
/// </summary>
public class DsnOption: Option<string> {

	/// <summary>
	/// Creates a new option.
	/// </summary>
	public DsnOption(): base(["-d", "--dsn"], "The connection string.") {
		IsRequired = true;
	}
}

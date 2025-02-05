namespace Belin.Cli.Data;

/// <summary>
/// Provides the metadata of a table column.
/// </summary>
public class Column {

	/// <summary>
	/// The name of the database table associated with this class.
	/// </summary>
	public const string TableName = "COLUMNS";

	/// <summary>
	/// The schema name.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The column position.
	/// </summary>
	public int Position { get; set; } = 0;

	/// <summary>
	/// The schema containing this column.
	/// </summary>
	public string Schema { get; set; } = string.Empty;

	/// <summary>
	/// The table containing this column.
	/// </summary>
	public string Table { get; set; } = string.Empty;
}

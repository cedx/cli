namespace Belin.Cli.MySql;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Provides the metadata of a table column.
/// </summary>
[Table("COLUMNS")]
public class Column {

	/// <summary>
	/// The column name.
	/// </summary>
	[Column("COLUMN_NAME")]
	public required string Name { get; init; }

	/// <summary>
	/// The column position.
	/// </summary>
	[Column("ORDINAL_POSITION")]
	public int Position { get; init; }

	/// <summary>
	/// The schema containing this column.
	/// </summary>
	[Column("TABLE_SCHEMA")]
	public string Schema { get; init; } = "";

	/// <summary>
	/// The table containing this column.
	/// </summary>
	[Column("TABLE_NAME")]
	public string Table { get; init; } = "";
}

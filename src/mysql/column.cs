namespace Belin.Cli.MySql;

using System.Data;

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

	/// <summary>
	/// Creates a new column from the specified database record.
	/// </summary>
	/// <param name="record">A database record providing values to initialize the instance.</param>
	/// <returns>The newly created column.</returns>
	public static Column OfRecord(IDataRecord record) {
		return new Column {
			Name = (string) record["COLUMN_NAME"] ?? string.Empty,
			Position = (int) record["ORDINAL_POSITION"],
			Schema = (string) record["TABLE_SCHEMA"] ?? string.Empty,
			Table = (string) record["TABLE_NAME"] ?? string.Empty
		};
	}
}

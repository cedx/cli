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
	/// The column name.
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// The column position.
	/// </summary>
	public int Position { get; init; } = 0;

	/// <summary>
	/// The schema containing this column.
	/// </summary>
	public string Schema { get; init; } = string.Empty;

	/// <summary>
	/// The table containing this column.
	/// </summary>
	public string Table { get; init; } = string.Empty;

	/// <summary>
	/// Creates a new column from the specified database record.
	/// </summary>
	/// <param name="record">A database record providing values to initialize the instance.</param>
	/// <returns>The newly created column.</returns>
	public static Column OfRecord(IDataRecord record) => new() {
		Name = record["COLUMN_NAME"] is DBNull ? string.Empty : (string) record["COLUMN_NAME"],
		Position = record["ORDINAL_POSITION"] is DBNull ? 0 : (int) record["ORDINAL_POSITION"],
		Schema = record["TABLE_SCHEMA"] is DBNull ? string.Empty : (string) record["TABLE_SCHEMA"],
		Table = record["TABLE_NAME"] is DBNull ? string.Empty : (string) record["TABLE_NAME"]
	};
}

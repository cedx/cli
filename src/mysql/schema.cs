namespace Belin.Cli.MySql;

using System.Data;

/// <summary>
/// Provides the metadata of a database schema.
/// </summary>
public class Schema {

	/// <summary>
	/// The name of the database table associated with this class.
	/// </summary>
	public const string TableName = "SCHEMATA";

	/// <summary>
	/// The default character set.
	/// </summary>
	public string Charset { get; init; } = string.Empty;

	/// <summary>
	/// The default collation.
	/// </summary>
	public string Collation { get; init; } = string.Empty;

	/// <summary>
	/// The schema name.
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// Creates a new column from the specified database record.
	/// </summary>
	/// <param name="record">A database record providing values to initialize the instance.</param>
	/// <returns>The newly created column.</returns>
	public static Schema OfRecord(IDataRecord record) => new() {
		Charset = record["DEFAULT_CHARACTER_SET_NAME"] is DBNull ? string.Empty : (string) record["DEFAULT_CHARACTER_SET_NAME"],
		Collation = record["DEFAULT_COLLATION_NAME"] is DBNull ? string.Empty : (string) record["DEFAULT_COLLATION_NAME"],
		Name = record["SCHEMA_NAME"] is DBNull ? string.Empty : (string) record["SCHEMA_NAME"]
	};
}

namespace Belin.Cli.MySql;

using System.Data;

/// <summary>
/// Provides the metadata of a database schema.
/// </summary>
public record Schema {

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
	public static Schema OfRecord(IDataRecord record) {
		return new Schema {
			Charset = (string?) record["DEFAULT_CHARACTER_SET_NAME"] ?? string.Empty,
			Collation = (string?) record["DEFAULT_COLLATION_NAME"] ?? string.Empty,
			Name = (string?) record["SCHEMA_NAME"] ?? string.Empty
		};
	}
}

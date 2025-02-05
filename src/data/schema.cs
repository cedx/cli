namespace Belin.Cli.Data;

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
	public string Charset { get; set; } = string.Empty;

	/// <summary>
	/// The default collation.
	/// </summary>
	public string Collation { get; set; } = string.Empty;

	/// <summary>
	/// The schema name.
	/// </summary>
	public required string Name { get; set; };
}

namespace Belin.Cli.MySql;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Provides the metadata of a database schema.
/// </summary>
[Table("SCHEMATA")]
public class Schema {

	/// <summary>
	/// The default character set.
	/// </summary>
	[Column("DEFAULT_CHARACTER_SET_NAME")]
	public string Charset { get; init; } = string.Empty;

	/// <summary>
	/// The default collation.
	/// </summary>
	[Column("DEFAULT_COLLATION_NAME")]
	public string Collation { get; init; } = string.Empty;

	/// <summary>
	/// The schema name.
	/// </summary>
	[Column("SCHEMA_NAME")]
	public required string Name { get; init; }
}

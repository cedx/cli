namespace Belin.Cli.MySql;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Provides the metadata of a database table.
/// </summary>
[Table("TABLES")]
public class Table {

	/// <summary>
	/// The default collation.
	/// </summary>
	[Column("TABLE_COLLATION")]
	public string Collation { get; set; } = "";

	/// <summary>
	/// The storage engine.
	/// </summary>
	[Column("ENGINE")]
	public string Engine { get; set; } = TableEngine.None;

	/// <summary>
	/// The table name.
	/// </summary>
	[Column("TABLE_NAME")]
	public string Name { get; set; } = "";

	/// <summary>
	/// The fully qualified name.
	/// </summary>
	public string QualifiedName => GetQualifiedName(escape: false);

	/// <summary>
	/// The schema containing this table.
	/// </summary>
	[Column("TABLE_SCHEMA")]
	public string Schema { get; set; } = "";

	/// <summary>
	/// The table type.
	/// </summary>
	[Column("TABLE_TYPE")]
	public string Type { get; set; } = TableType.BaseTable;

	/// <summary>
	/// Gets the fully qualified name.
	/// </summary>
	/// <param name="escape">Value indicating whether to escape the SQL identifiers.</param>
	/// <returns>The fully qualified name.</returns>
	public string GetQualifiedName(bool escape = false) {
		Func<string, string> escapeFunc = escape ? identifier => $"`{identifier}`" : identifier => identifier;
		return $"{escapeFunc(Schema)}.{escapeFunc(Name)}";
	}
}

/// <summary>
/// Defines the storage engine of a table.
/// </summary>
public static class TableEngine {

	/// <summary>
	/// The table does not use any storage engine.
	/// </summary>
	public const string None = "";

	/// <summary>
	/// The storage engine is Aria.
	/// </summary>
	public const string Aria = "Aria";

	/// <summary>
	/// The storage engine is InnoDB
	/// </summary>
	public const string InnoDB = "InnoDB";

	/// <summary>
	/// The storage engine is MyISAM.
	/// </summary>
	public const string MyISAM = "MyISAM";
}

/// <summary>
/// Defines the type of a table.
/// </summary>
public static class TableType {

	/// <summary>
	/// A base table.
	/// </summary>
	public const string BaseTable = "BASE TABLE";

	/// <summary>
	/// A view.
	/// </summary>
	public const string View = "VIEW";
}

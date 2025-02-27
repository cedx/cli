namespace Belin.Cli.MySql;

using System.Data;

/// <summary>
/// Provides the metadata of a database table.
/// </summary>
public class Table {

	/// <summary>
	/// The name of the database table associated with this class.
	/// </summary>
	public const string TableName = "TABLES";

	/// <summary>
	/// The default collation.
	/// </summary>
	public string Collation { get; set; } = string.Empty;

	/// <summary>
	/// The storage engine.
	/// </summary>
	public string Engine { get; set; } = TableEngine.None;

	/// <summary>
	/// The schema name.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The fully qualified name.
	/// </summary>
	public string QualifiedName { get => GetQualifiedName(); }

	/// <summary>
	/// The schema containing this table.
	/// </summary>
	public string Schema { get; set; } = string.Empty;

	/// <summary>
	/// The table type.
	/// </summary>
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

	/// <summary>
	/// Creates a new column from the specified database record.
	/// </summary>
	/// <param name="record">A database record providing values to initialize the instance.</param>
	/// <returns>The newly created column.</returns>
	public static Table OfRecord(IDataRecord record) {
		return new Table {
			Collation = (string) record["TABLE_COLLATION"] ?? string.Empty,
			Engine = (string) record["ENGINE"] ?? string.Empty,
			Name = (string) record["TABLE_NAME"] ?? string.Empty,
			Schema = (string) record["TABLE_SCHEMA"] ?? string.Empty,
			Type = (string) record["TABLE_TYPE"] ?? string.Empty
		};
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

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
	public string Collation { get; init; } = string.Empty;

	/// <summary>
	/// The storage engine.
	/// </summary>
	public string Engine { get; init; } = TableEngine.None;

	/// <summary>
	/// The table name.
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// The fully qualified name.
	/// </summary>
	public string QualifiedName => GetQualifiedName();

	/// <summary>
	/// The schema containing this table.
	/// </summary>
	public string Schema { get; init; } = string.Empty;

	/// <summary>
	/// The table type.
	/// </summary>
	public string Type { get; init; } = TableType.BaseTable;

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
	public static Table OfRecord(IDataRecord record) => new() {
		Collation = record["TABLE_COLLATION"] is DBNull ? string.Empty : (string) record["TABLE_COLLATION"],
		Engine = record["ENGINE"] is DBNull ? TableEngine.None : (string) record["ENGINE"],
		Name = record["TABLE_NAME"] is DBNull ? string.Empty : (string) record["TABLE_NAME"],
		Schema = record["TABLE_SCHEMA"] is DBNull ? string.Empty : (string) record["TABLE_SCHEMA"],
		Type = record["TABLE_TYPE"] is DBNull ? TableType.BaseTable : (string) record["TABLE_TYPE"]
	};
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

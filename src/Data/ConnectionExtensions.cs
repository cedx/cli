namespace Belin.Cli.MySql;

using namespace System.Data;

/// <summary>
/// Provides extension methods for database connections.
/// </summary>
class ConnectionExtensions {

	/// <summary>
	/// Gets the list of columns contained in the specified table.
	/// </summary>
	/// <param name="connection">The database connectinon.</param>
	/// <param name="table">The database table.</param>
	/// <returns>The columns contained in the specified table.</returns>
	public static IEnumerable<Column> GetColumns(this IDbConnection connection, Table table) {
		var sql = """
			SELECT *
			FROM information_schema.COLUMNS
			WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @Name
			ORDER BY ORDINAL_POSITION
			""";

		return connection.Query<Column>(sql, new { table.Schema, table.Name });
	}

	/// <summary>
	/// Gets the list of schemas hosted by this database.
	/// </summary>
	/// <param name="connection">The database connectinon.</param>
	/// <returns>The schemas hosted by the database.</returns>
	public static IEnumerable<Schema> GetSchemas(this IDbConnection connection) =>
		connection.Query<Schema>("""
			SELECT *
			FROM information_schema.SCHEMATA
			WHERE SCHEMA_NAME NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')
			ORDER BY SCHEMA_NAME
			""");

	/// <summary>
	/// Gets the list of tables contained in the specified schema.
	/// </summary>
	/// <param name="connection">The database connectinon.</param>
	/// <param name="schema">The database schema.</param>
	/// <returns>The tables contained in the specified schema.</returns>
	public static IEnumerable<Table> GetTables(this IDbConnection connection, Schema schema) {
		var sql = """
			SELECT *
			FROM information_schema.TABLES
			WHERE TABLE_SCHEMA = @Name AND TABLE_TYPE = @Type
			ORDER BY TABLE_NAME
			""";

		return connection.Query<Table>(sql, new { schema.Name, Type = TableType.BaseTable });
	}
}

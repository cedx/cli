namespace Belin.Cli.MySql;

using MySqlConnector;

/// <summary>
/// Provides extension methods for commands.
/// </summary>
public static class ConnectionExtensions {

	/// <summary>
	/// Gets the list of columns contained in the specified table.
	/// </summary>
	/// <param name="connection">The database connectinon.</param>
	/// <param name="table">The database table.</param>
	/// <returns>The columns contained in the specified table.</returns>
	public static IList<Column> GetColumns(this MySqlConnection connection, Table table) {
		using var command = connection.CreateCommand();
		command.Parameters.AddWithValue("@name", table.Name);
		command.Parameters.AddWithValue("@schema", table.Schema);
		command.CommandText = """
			SELECT *
			FROM information_schema.COLUMNS
			WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @name
			ORDER BY ORDINAL_POSITION
		""";

		using var reader = command.ExecuteReader();
		var columns = new List<Column>();
		while (reader.Read()) columns.Add(Column.OfRecord(reader));
		return columns;
	}

	/// <summary>
	/// Gets the list of schemas hosted by this database.
	/// </summary>
	/// <param name="connection">The database connectinon.</param>
	/// <returns>The schemas hosted by the database.</returns>
	public static IList<Schema> GetSchemas(this MySqlConnection connection) {
		using var command = connection.CreateCommand();
		command.CommandText = """
			SELECT *
			FROM information_schema.SCHEMATA
			WHERE SCHEMA_NAME NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')
			ORDER BY SCHEMA_NAME
		""";

		using var reader = command.ExecuteReader();
		var schemas = new List<Schema>();
		while (reader.Read()) schemas.Add(Schema.OfRecord(reader));
		return schemas;
	}

	/// <summary>
	/// Gets the list of tables contained in the specified schema.
	/// </summary>
	/// <param name="connection">The database connectinon.</param>
	/// <param name="schema">The database schema.</param>
	/// <returns>The tables contained in the specified schema.</returns>
	public static IList<Table> GetTables(this MySqlConnection connection, Schema schema) {
		using var command = connection.CreateCommand();
		command.Parameters.AddWithValue("@schema", schema.Name);
		command.Parameters.AddWithValue("@type", TableType.BaseTable);
		command.CommandText = """
			SELECT *
			FROM information_schema.TABLES
			WHERE TABLE_SCHEMA = @schema AND TABLE_TYPE = @type
			ORDER BY TABLE_NAME
		""";

		using var reader = command.ExecuteReader();
		var tables = new List<Table>();
		while (reader.Read()) tables.Add(Table.OfRecord(reader));
		return tables;
	}
}

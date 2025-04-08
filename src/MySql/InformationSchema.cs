namespace Belin.Cli.MySql;

using Dapper;
using MySqlConnector;
using System.Data;

/// <summary>
/// Represents a session to TODO.
/// </summary>
public sealed class InformationSchema {

	/// <summary>
	/// The list of local host names.
	/// </summary>
	private readonly string[] localHosts = ["::1", "127.0.0.1", "localhost"];

	/// <summary>
	/// Initializes this class.
	/// </summary>
	static InformationSchema() {
		SqlMapper.SetTypeMap(typeof(Column), new ColumnAttributeTypeMap<Column>());
		SqlMapper.SetTypeMap(typeof(Table), new ColumnAttributeTypeMap<Table>());
	}

	/// <summary>
	/// Creates and opens a new database connection.
	/// </summary>
	/// <param name="uri">The connection URI used to connect to the database.</param>
	/// <returns>The newly created database connection.</returns>
	public IDbConnection OpenConnection(Uri uri) {
		var userInfo = uri.UserInfo.Split(':').Select(Uri.UnescapeDataString);
		var builder = new MySqlConnectionStringBuilder {
			Server = uri.Host,
			Port = uri.IsDefaultPort ? 3306 : (uint) uri.Port,
			Database = "information_schema",
			UserID = userInfo.First(),
			Password = userInfo.Last(),
			ConvertZeroDateTime = true, // TODO: probably not needed...
			Pooling = false,
			UseCompression = !localHosts.Contains(uri.Host)
		};

		var connection = new MySqlConnection(builder.ConnectionString);
		connection.Open();
		return connection;
	}
}

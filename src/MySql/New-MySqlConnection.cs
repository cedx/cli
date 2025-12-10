namespace Belin.Cli.MySql;

using MySqlConnector;

/// <summary>
/// Creates a new MariaDB/MySQL database connection.
/// </summary>
[Cmdlet(VerbsCommon.New, "MySqlConnection"), OutputType(typeof(MySqlConnection))]
public class NewMySqlConnectionCommand: Cmdlet {

	/// <summary>
	/// The local host list.
	/// </summary>
	private static readonly string[] localHosts = ["::1", "127.0.0.1", "localhost"];

	/// <summary>
	/// Value indicating whether to open the connection.
	/// </summary>
	[Parameter]
	public SwitchParameter Open { get; set; }

	/// <summary>
	/// The connection URI used to open the database.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required Uri Uri { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() {
		var userInfo = Uri.UserInfo.Split(':').Select(Uri.UnescapeDataString);
		var builder = new MySqlConnectionStringBuilder {
			Server = Uri.Host,
			Port = Uri.IsDefaultPort ? 3306 : (uint) Uri.Port,
			Database = "information_schema",
			UserID = userInfo.First(),
			Password = userInfo.Last(),
			ConvertZeroDateTime = true,
			Pooling = false,
			UseCompression = !localHosts.Contains(Uri.Host)
		};

		var connection = new MySqlConnection(builder.ConnectionString);
		if (Open) connection.Open();
		WriteObject(connection);
	}
}

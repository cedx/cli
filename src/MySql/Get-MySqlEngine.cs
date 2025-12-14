namespace Belin.Cli.MySql;

using Belin.Sql;
using System.Data;

/// <summary>
/// Gets the list of all storage engines.
/// </summary>
[Cmdlet(VerbsCommon.Get, "MySqlEngine"), OutputType(typeof(string[]))]
public class GetMySqlEngineCommand: Cmdlet {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required IDbConnection Connection { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() {
		WriteObject(Connection.Query("SHOW ENGINES").Select(row => row.Engine).ToArray());
	}
}

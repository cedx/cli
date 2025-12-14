namespace Belin.Cli.MySql;

using Belin.Sql;
using System.Data;

/// <summary>
/// Gets the list of all collations.
/// </summary>
[Cmdlet(VerbsCommon.Get, "MySqlCollation"), OutputType(typeof(string[]))]
public class GetMySqlCollationCommand: Cmdlet {
		
	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required IDbConnection Connection { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() {
		WriteObject(Connection.Query("SHOW COLLATION").Select(row => row.Collation).ToArray());
	}
}

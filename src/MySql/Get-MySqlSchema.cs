namespace Belin.Cli.MySql;

using Belin.Sql;
using System.Data;

/// <summary>
/// Gets the list of schemas hosted by a database server.
/// </summary>
[Cmdlet(VerbsCommon.Get, "MySqlSchema"), OutputType(typeof(Schema[]))]
public class GetMySqlSchemaCommand: Cmdlet {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required IDbConnection Connection { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() {
		var sql = """
			SELECT *
			FROM information_schema.SCHEMATA
			WHERE SCHEMA_NAME NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')
			ORDER BY SCHEMA_NAME
			""";

		WriteObject(Connection.Query<Schema>(sql).ToArray());
	}
}

namespace Belin.Cli.MySql;

using Belin.Sql;
using System.Data;

/// <summary>
/// Gets the list of columns contained in the specified table.
/// </summary>
[Cmdlet(VerbsCommon.Get, "MySqlColumn"), OutputType(typeof(Column))]
public class GetMySqlColumnCommand: Cmdlet {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required IDbConnection Connection { get; set; }

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 1)]
	public required Table Table { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() {
		var sql = """
			SELECT *
			FROM information_schema.COLUMNS
			WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @Name
			ORDER BY ORDINAL_POSITION
			""";

		WriteObject(Connection.Query<Column>(sql, [
			("Name", Table.Name),
			("Schema", Table.Schema)
		]), enumerateCollection: true);
	}
}

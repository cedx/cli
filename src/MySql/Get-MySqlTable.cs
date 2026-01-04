namespace Belin.Cli.MySql;

using System.Data;

/// <summary>
/// Gets the list of tables contained in the specified schema.
/// </summary>
[Cmdlet(VerbsCommon.Get, "MySqlTable"), OutputType(typeof(Table))]
public class GetMySqlTableCommand: Cmdlet {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required IDbConnection Connection { get; set; }

	/// <summary>
	/// The database schema.
	/// </summary>
	[Parameter(Mandatory = true, Position = 1)]
	public required Schema Schema { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() {
		var sql = """
			SELECT *
			FROM information_schema.TABLES
			WHERE TABLE_SCHEMA = @Name AND TABLE_TYPE = @Type
			ORDER BY TABLE_NAME
			""";

		WriteObject(Connection.Query<Table>(sql, [
			("Name", Schema.Name),
			("Type", TableType.BaseTable)
		]), enumerateCollection: true);
	}
}

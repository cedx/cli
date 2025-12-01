using namespace Belin.Cli.MySql
using namespace MySqlConnector

<#
.SYNOPSIS
	Gets the list of tables contained in the specified schema.
.PARAMETER Connection
	The connection to the data source.
.PARAMETER Schema
	The database schema.
.OUTPUTS
	The tables contained in the specified schema.
#>
function Get-MySqlTable {
	[OutputType([Table[]])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[MySqlConnection] $Connection,

		[Parameter(Mandatory, Position = 1)]
		[Schema] $Schema
	)

	$sql = "
		SELECT *
		FROM information_schema.TABLES
		WHERE TABLE_SCHEMA = @Name AND TABLE_TYPE = @Type
		ORDER BY TABLE_NAME"

	Invoke-SqlQuery $Connection -As ([Table]) -Command $sql -Parameters @{
		Name = $Schema.Name
		Type = [TableType]::BaseTable
	}
}

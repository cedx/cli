using namespace Belin.Cli.MySql
using namespace MySqlConnector

<#
.SYNOPSIS
	Gets the list of columns contained in the specified table.
.PARAMETER Connection
	The connection to the data source.
.PARAMETER Table
	The database table.
.OUTPUTS
	The columns contained in the specified table.
#>
function Get-MySqlColumn {
	[OutputType([Column[]])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[MySqlConnection] $Connection,

		[Parameter(Mandatory, Position = 1)]
		[Table] $Table
	)

	$sql = "
		SELECT *
		FROM information_schema.COLUMNS
		WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @Name
		ORDER BY ORDINAL_POSITION"

	Invoke-SqlQuery $Connection -As ([Column]) -Command $sql -Parameters @{
		Name = $Table.Name
		Schema = $Table.Schema
	}
}

using namespace MySqlConnector
using module ./Column.psm1
using module ./Table.psm1

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
function Get-Column {
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

	$records = Invoke-DapperQuery $Connection -Command $sql -Parameters @{
		Name = $Table.Name
		Schema = $Table.Schema
	}

	$records.ForEach{
		[Column]@{
			Name = $_.COLUMN_NAME
			Position = $_.ORDINAL_POSITION
			Schema = $_.TABLE_SCHEMA
			Table = $_.TABLE_NAME
		}
	}
}

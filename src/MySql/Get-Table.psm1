using namespace MySqlConnector
using module ./Schema.psm1
using module ./Table.psm1

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
function Get-Table {
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

	$records = Invoke-DapperQuery $Connection -Command $sql -Parameters @{
		Name = $Schema.Name
		Type = [TableType]::BaseTable
	}

	$records.ForEach{
		[Table]@{
			Collation = $_.TABLE_COLLATION
			Engine = $_.ENGINE
			Name = $_.TABLE_NAME
			Schema = $_.TABLE_SCHEMA
			Type = $_.TABLE_TYPE
		}
	}
}

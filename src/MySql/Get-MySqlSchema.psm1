using namespace MySqlConnector
using module ./Schema.psm1

<#
.SYNOPSIS
	Gets the list of schemas hosted by a database server.
.PARAMETER Connection
	The connection to the data source.
.OUTPUTS
	The schemas hosted by the database server.
#>
function Get-MySqlSchema {
	[OutputType([Schema[]])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[MySqlConnection] $Connection
	)

	$sql = "
		SELECT *
		FROM information_schema.SCHEMATA
		WHERE SCHEMA_NAME NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')
		ORDER BY SCHEMA_NAME"

	$records = Invoke-SqlQuery $Connection -Command $sql
	$records.ForEach{
		[Schema]@{
			Charset = $_.DEFAULT_CHARACTER_SET_NAME
			Collation = $_.DEFAULT_COLLATION_NAME
			Name = $_.SCHEMA_NAME
		}
	}
}

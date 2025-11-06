using namespace MySqlConnector
using namespace System.Collections.Generic
using module ./Schema.psm1

<#
.SYNOPSIS
	Gets the list of schemas hosted by a database server.
.PARAMETER Connection
	The database connection.
.OUTPUTS
	The schemas hosted by the database server.
#>
function Get-Schema {
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

	$list = [List[Schema]]::new()
	$reader = [MySqlCommand]::new($sql, $Connection).ExecuteReader()
	while ($reader.Read()) { $list.Add([Schema]::new($reader)) }
	$reader.Close()
	$list.ToArray()
}

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
	param ([Parameter(Mandatory, Position = 0)] [ValidateNotNull()] [MySqlConnection] $Connection)

	$sql = "
		SELECT *
		FROM information_schema.SCHEMATA
		WHERE SCHEMA_NAME NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')
		ORDER BY SCHEMA_NAME"

	$command = [MySqlCommand]::new($sql, $Connection)
	$reader = $command.ExecuteReader()

	$list = [List[Schema]]::new()
	while ($reader.Read()) { $list.Add([Schema]::OfRecord($reader)) }
	$reader.Close()
	$list.ToArray()
}

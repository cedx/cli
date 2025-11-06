using namespace MySqlConnector
using namespace System.Collections.Generic

<#
.SYNOPSIS
	Gets the list of all storage engines.
.PARAMETER Connection
	The database connection.
.OUTPUTS
	The list of all storage engines.
#>
function Get-Engine {
	[OutputType([string[]])]
	param ([Parameter(Mandatory, Position = 0)] [ValidateNotNull()] [MySqlConnection] $Connection)

	$list = [List[string]]::new()
	$reader = [MySqlCommand]::new("SHOW ENGINES", $Connection).ExecuteReader()
	while ($reader.Read()) { $list.Add($reader["Engine"]) }
	$reader.Close()
	$list.ToArray()
}

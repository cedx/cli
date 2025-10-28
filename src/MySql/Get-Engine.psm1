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

	$command = [MySqlCommand]::new("SHOW ENGINES", $Connection)
	$reader = $command.ExecuteReader()

	$list = [List[string]]::new()
	while ($reader.Read()) { $list.Add($reader["Engine"]) }
	$reader.Close()
	$list.ToArray()
}

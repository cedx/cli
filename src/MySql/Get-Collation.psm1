using namespace System.Collections.Generic

<#
.SYNOPSIS
	Gets the list of all collations.
.PARAMETER Connection
	The database connection.
.OUTPUTS
	The list of all collations.
#>
function Get-Collation {
	[OutputType([string[]])]
	param ([Parameter(Mandatory, Position = 0)] [ValidateNotNull()] [MySqlConnection] $Connection)

	$command = [MySqlCommand]::new("SHOW COLLATION", $Connection)
	$reader = $command.ExecuteReader()

	$list = [List[string]]::new()
	while ($reader.Read()) { $list.Add($reader["Collation"]) }
	$reader.Close()
	$list.ToArray()
}

using namespace MySqlConnector

<#
.SYNOPSIS
	Gets the list of all storage engines.
.PARAMETER Connection
	The connection to the data source.
.OUTPUTS
	The list of all storage engines.
#>
function Get-Engine {
	[OutputType([string[]])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[MySqlConnection] $Connection
	)

	$records = Invoke-DapperQuery $Connection -Command "SHOW ENGINES"
	$records.ForEach{ $_.Engine }
}

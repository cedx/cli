using namespace MySqlConnector

<#
.SYNOPSIS
	Gets the list of all collations.
.PARAMETER Connection
	The connection to the data source.
.OUTPUTS
	The list of all collations.
#>
function Get-MySqlCollation {
	[OutputType([string[]])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[MySqlConnection] $Connection
	)

	$records = Invoke-SqlQuery $Connection -Command "SHOW COLLATION"
	$records.ForEach{ $_.Collation }
}

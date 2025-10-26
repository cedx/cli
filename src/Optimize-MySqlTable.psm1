using module ./Data/New-MySqlConnection.psm1

<#
.SYNOPSIS
	Optimizes a set of MariaDB/MySQL tables.
.PARAMETER Uri
	The connection URI.
.PARAMETER Schema
	The schema name.
.PARAMETER Table
	The table names (requires a schema).
#>
function Optimize-MySqlTable {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[uri] $Uri,

		[Parameter()]
		[string] $Schema = "",

		[Parameter()]
		[string[]] $Table = @()
	)

	$noSchema = [string]::IsNullOrWhiteSpace($Schema)
	if ($noSchema -and ($Table.Count -gt 0)) { throw [InvalidOperationException] "The table ""$($Table[0])"" requires that a schema be specified." }

	$connection = $null
	try {
		$connection = New-MySqlConnection $Uri -Open




		$connection.Close()
	}
	finally {
		${connection}?.Dispose()
	}
}

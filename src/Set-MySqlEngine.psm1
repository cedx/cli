using module ./Data/New-Connection.psm1

<#
.SYNOPSIS
	Alters the storage engine of MariaDB/MySQL tables.
.PARAMETER Uri
	The connection URI.
.PARAMETER Schema
	The schema name.
.PARAMETER Table
	The table names (requires a schema).
#>
function Set-MySqlEngine {
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
	if ($noSchema -and $Table) { throw [InvalidOperationException] "The table ""$($Table[0])"" requires that a schema be specified." }

	$connection = $null
	try {
		$connection = New-Connection $Uri -Open

	}
	finally {
		${connection}?.Dispose()
	}
}

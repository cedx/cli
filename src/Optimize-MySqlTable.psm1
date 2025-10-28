using namespace MySqlConnector
using module ./Data/Get-MySqlSchemas.psm1
using module ./Data/Get-MySqlTables.psm1
using module ./Data/Invoke-NonQuery.psm1
using module ./Data/New-MySqlConnection.psm1
using module ./Data/Schema.psm1
using module ./Data/Table.psm1

<#
.SYNOPSIS
	Optimizes a set of MariaDB/MySQL tables.
.PARAMETER Uri
	The connection URI.
.PARAMETER Schema
	The schema name.
.PARAMETER Table
	The table name.
#>
function Optimize-MySqlTable {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[ValidateNotNull()]
		[uri] $Uri,

		[Parameter()]
		[string[]] $Schema = @(),

		[Parameter()]
		[string[]] $Table = @()
	)

	$connection = $null
	try {
		$connection = New-MySqlConnection $Uri -Open
		$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-MySqlSchemas $connection)
		$tables = foreach ($schemaObject in $schemas) {
			$Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $schemaObject.Name } } : (Get-MySqlTables $connection $schemaObject)
		}

		foreach ($tableObject in $tables) {
			"Optimizing: $($tableObject.QualifiedName($false))"
			$command = [MySqlCommand]::new("OPTIMIZE TABLE $($tableObject.QualifiedName($true))", $Connection)
			$result = Invoke-NonQuery $command
			if ($result.IsFailure) { Write-Error ($result.Message ? $result.Message : "An error occurred.") }
		}

		$connection.Close()
	}
	finally {
		${connection}?.Dispose()
	}
}

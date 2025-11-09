using namespace MySqlConnector
using namespace System.Diagnostics.CodeAnalysis
using module ./MySql/Get-Schema.psm1
using module ./MySql/Get-Table.psm1
using module ./MySql/Invoke-NonQuery.psm1
using module ./MySql/New-MySqlConnection.psm1
using module ./MySql/Schema.psm1
using module ./MySql/Table.psm1

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
	[SuppressMessage("PSUseOutputTypeCorrectly", "")]
	param (
		[Parameter(Mandatory, Position = 0)]
		[uri] $Uri,

		[Parameter()]
		[string[]] $Schema = @(),

		[Parameter()]
		[string[]] $Table = @()
	)

	$connection = $null
	try {
		$connection = New-MySqlConnection $Uri -Open
		$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-Schema $connection)
		$tables = foreach ($schemaObject in $schemas) {
			$Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $schemaObject.Name } } : (Get-Table $connection $schemaObject)
		}

		foreach ($tableObject in $tables) {
			"Optimizing: $($tableObject.GetQualifiedName($false))"
			$command = [MySqlCommand]::new("OPTIMIZE TABLE $($tableObject.GetQualifiedName($true))", $connection)
			$result = Invoke-NonQuery $command
			if ($result.IsFailure) { Write-Error ($result.Message ? $result.Message : "An error occurred.") }
		}

		$connection.Close()
	}
	finally {
		${connection}?.Dispose()
	}
}

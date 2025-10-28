using namespace MySqlConnector
using module ./MySql/Get-Engine.psm1
using module ./MySql/Get-Schema.psm1
using module ./MySql/Get-Table.psm1
using module ./MySql/Invoke-NonQuery.psm1
using module ./MySql/New-Connection.psm1
using module ./MySql/Schema.psm1
using module ./MySql/Table.psm1

<#
.SYNOPSIS
	Alters the storage engine of MariaDB/MySQL tables.
.PARAMETER Uri
	The connection URI.
.PARAMETER Engine
	The name of the new storage engine.
.PARAMETER Schema
	The schema name.
.PARAMETER Table
	The table name.
#>
function Set-MySqlEngine {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[ValidateNotNull()]
		[uri] $Uri,

		[Parameter(Mandatory, Position = 1)]
		[ValidateNotNullOrWhiteSpace()]
		[string] $Engine,

		[Parameter()]
		[string[]] $Schema = @(),

		[Parameter()]
		[string[]] $Table = @()
	)

	$connection = $null
	try {
		$connection = New-Connection $Uri -Open
		$engines = Get-Engine $connection
		if ($Engine -notin $engines) { throw [ArgumentOutOfRangeException] "Engine" }

		$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-Schema $connection)
		$tables = foreach ($schemaObject in $schemas) {
			$Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $schemaObject.Name } } : (Get-Table $connection $schemaObject)
		}

		foreach ($tableObject in $tables) {
			"Processing: $($tableObject.QualifiedName($false))"
			[MySqlCommand]::new("SET foreign_key_checks = 0", $Connection).ExecuteNonQuery() | Out-Null

			$command = [MySqlCommand]::new("ALTER TABLE $($tableObject.QualifiedName($true)) ENGINE = $Engine", $Connection)
			$result = Invoke-NonQuery $command
			if ($result.IsFailure) { Write-Error ($result.Message ? $result.Message : "An error occurred.") }

			[MySqlCommand]::new("SET foreign_key_checks = 1", $Connection).ExecuteNonQuery() | Out-Null
		}

		$connection.Close()
	}
	finally {
		${connection}?.Dispose()
	}
}

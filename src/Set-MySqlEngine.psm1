using namespace MySqlConnector
using namespace System.Diagnostics.CodeAnalysis
using module ./MySql/Get-MySqlEngine.psm1
using module ./MySql/Get-MySqlSchema.psm1
using module ./MySql/Get-MySqlTable.psm1
using module ./MySql/Invoke-NonQuery.psm1
using module ./MySql/New-MySqlConnection.psm1
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
	[SuppressMessage("PSUseOutputTypeCorrectly", "")]
	[SuppressMessage("PSUseShouldProcessForStateChangingFunctions", "")]
	param (
		[Parameter(Mandatory, Position = 0)]
		[uri] $Uri,

		[Parameter(Mandatory, Position = 1)]
		[string] $Engine,

		[Parameter()]
		[string[]] $Schema = @(),

		[Parameter()]
		[string[]] $Table = @()
	)

	$connection = New-MySqlConnection $Uri -Open
	$engines = Get-MySqlEngine $connection
	if ($Engine -notin $engines) { throw [ArgumentOutOfRangeException] "Engine" }

	$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-MySqlSchema $connection)
	$tables = foreach ($schemaObject in $schemas) {
		$Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $schemaObject.Name } } : (Get-MySqlTable $connection $schemaObject)
	}

	foreach ($tableObject in $tables) {
		"Processing: $($tableObject.GetQualifiedName($false))"
		[MySqlCommand]::new("SET foreign_key_checks = 0", $connection).ExecuteNonQuery() | Out-Null

		$command = [MySqlCommand]::new("ALTER TABLE $($tableObject.GetQualifiedName($true)) ENGINE = $Engine", $connection)
		$result = Invoke-NonQuery $command
		if ($result.IsFailure) { Write-Error ($result.Message ? $result.Message : "An error occurred.") }

		[MySqlCommand]::new("SET foreign_key_checks = 1", $connection).ExecuteNonQuery() | Out-Null
	}

	Close-DapperConnection $connection
}

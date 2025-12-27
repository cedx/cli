using namespace Belin.Cli.MySql
using namespace MySqlConnector
using namespace System.Diagnostics.CodeAnalysis

<#
.SYNOPSIS
	Alters the storage engine of MariaDB/MySQL tables.
.OUTPUTS
	The log messages.
#>
function Set-MySqlEngine {
	[CmdletBinding()]
	[OutputType([string])]
	[SuppressMessage("PSUseShouldProcessForStateChangingFunctions", "")]
	param (
		# The connection URI.
		[Parameter(Mandatory, Position = 0)]
		[uri] $Uri,

		# The name of the new storage engine.
		[Parameter(Mandatory, Position = 1)]
		[string] $Engine,

		# The schema name.
		[Parameter()]
		[string[]] $Schema = @(),

		# The table name.
		[Parameter()]
		[string[]] $Table = @()
	)

	$connection = New-MySqlConnection $Uri
	$engines = Get-MySqlEngine $connection
	if ($Engine -notin $engines) { throw [ArgumentOutOfRangeException] "Engine" }

	$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-MySqlSchema $connection)
	$tables = foreach ($schemaObject in $schemas) {
		$Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $schemaObject.Name } } : (Get-MySqlTable $connection $schemaObject)
	}

	foreach ($tableObject in $tables) {
		"Processing: $($tableObject.GetQualifiedName($false))"
		Invoke-SqlNonQuery $connection -Command "SET foreign_key_checks = 0" | Out-Null
		Invoke-SqlNonQuery $connection -Command "ALTER TABLE $($tableObject.GetQualifiedName($true)) ENGINE = $Engine" | Out-Null
		Invoke-SqlNonQuery $connection -Command "SET foreign_key_checks = 1" | Out-Null
	}

	Close-SqlConnection $connection
}

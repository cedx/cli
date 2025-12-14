using namespace Belin.Cli.MySql
using namespace MySqlConnector
using namespace System.Diagnostics.CodeAnalysis

<#
.SYNOPSIS
	Alters the character set of MariaDB/MySQL tables.
.PARAMETER Uri
	The connection URI.
.PARAMETER Collation
	The name of the new character set.
.PARAMETER Schema
	The schema name.
.PARAMETER Table
	The table name.
.OUTPUTS
	The log messages.
#>
function Set-MySqlCharset {
	[CmdletBinding()]
	[OutputType([string])]
	[SuppressMessage("PSUseShouldProcessForStateChangingFunctions", "")]
	param (
		[Parameter(Mandatory, Position = 0)]
		[uri] $Uri,

		[Parameter(Mandatory, Position = 1)]
		[string] $Collation,

		[Parameter()]
		[string[]] $Schema = @(),

		[Parameter()]
		[string[]] $Table = @()
	)

	$connection = New-MySqlConnection $Uri
	$collations = Get-MySqlCollation $connection
	if ($Collation -notin $collations) { throw [ArgumentOutOfRangeException] "Collation" }

	$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-MySqlSchema $connection)
	$tables = foreach ($schemaObject in $schemas) {
		$Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $schemaObject.Name } } : (Get-MySqlTable $connection $schemaObject)
	}

	foreach ($tableObject in $tables) {
		"Processing: $($tableObject.GetQualifiedName($false))"
		$charset = ($Collation -split "_")[0]
		Invoke-SqlNonQuery $connection -Command "SET foreign_key_checks = 0" | Out-Null
		Invoke-SqlNonQuery $connection -Command "ALTER TABLE $($tableObject.GetQualifiedName($true)) CONVERT TO CHARACTER SET $charset COLLATE $Collation" | Out-Null
		Invoke-SqlNonQuery $connection -Command "SET foreign_key_checks = 1" | Out-Null
	}

	Close-SqlConnection $connection
}

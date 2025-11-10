using namespace MySqlConnector
using namespace System.Diagnostics.CodeAnalysis
using module ./MySql/Get-MySqlSchema.psm1
using module ./MySql/Get-MySqlTable.psm1
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

	$connection = New-MySqlConnection $Uri -Open
	$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-MySqlSchema $connection)
	$tables = foreach ($schemaObject in $schemas) {
		$Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $schemaObject.Name } } : (Get-MySqlTable $connection $schemaObject)
	}

	foreach ($tableObject in $tables) {
		"Optimizing: $($tableObject.GetQualifiedName($false))"
		Invoke-SqlNonQuery $connection -Command "OPTIMIZE TABLE $($tableObject.GetQualifiedName($true))" | Out-Null
	}

	Close-SqlConnection $connection
}

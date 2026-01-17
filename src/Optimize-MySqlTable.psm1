using namespace Belin.Cli.MySql
using namespace System.Diagnostics.CodeAnalysis

<#
.SYNOPSIS
	Optimizes a set of MariaDB/MySQL tables.
.OUTPUTS
	The log messages.
#>
function Optimize-MySqlTable {
	[CmdletBinding()]
	[OutputType([string])]
	param (
		# The connection URI.
		[Parameter(Mandatory, Position = 0)]
		[uri] $Uri,

		# The schema name.
		[Parameter()]
		[string[]] $Schema = @(),

		# The table name.
		[Parameter()]
		[string[]] $Table = @()
	)

	begin {
		$connection = New-MySqlConnection $Uri
	}

	process {
		$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-MySqlSchema $connection)
		$tables = foreach ($schemaObject in $schemas) {
			$Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $schemaObject.Name } } : (Get-MySqlTable $connection $schemaObject)
		}

		foreach ($tableObject in $tables) {
			"Optimizing: $($tableObject.GetQualifiedName($false))"
			Invoke-SqlNonQuery $connection -Command "OPTIMIZE TABLE $($tableObject.GetQualifiedName($true))" | Out-Null
		}
	}

	clean {
		Close-SqlConnection $connection
	}
}

using module ./Get-MySqlSchema.psm1
using module ./Get-MySqlTable.psm1
using module ./New-MySqlConnection.psm1
using module ./Schema.psm1

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
		[string[]] $Schema = @(),

		# The table name.
		[string[]] $Table = @()
	)

	begin {
		$connection = New-MySqlConnection $Uri
	}

	process {
		$schemas = $Schema ? $Schema.ForEach{ [Schema]@{ Name = $_ } } : @(Get-MySqlSchema $connection)
		$tables = $schemas | ForEach-Object {
			$schemaObject = $_
			$Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $schemaObject.Name } } : (Get-MySqlTable $connection $schemaObject)
		}

		$tables | ForEach-Object {
			"Optimizing: $($_.GetQualifiedName($false))"
			Invoke-SqlNonQuery $connection -Command "OPTIMIZE TABLE $($_.GetQualifiedName($true))" | Out-Null
		}
	}

	clean {
		Close-SqlConnection $connection
	}
}

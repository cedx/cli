using namespace Belin.Cli.MySql

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
		$schemas = $Schema ? ($Schema | ForEach-Object { [Schema]@{ Name = $_ } }) : (Get-MySqlSchema $connection)
		$tables = $schemas | ForEach-Object {
			$schemaObject = $_
			$Table ? ($Table | ForEach-Object { [Table]@{ Name = $_; Schema = $schemaObject.Name } }) : (Get-MySqlTable $connection $schemaObject)
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

using namespace Belin.Cli.MySql
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

	begin {
		$connection = New-MySqlConnection $Uri
		$engines = Get-MySqlEngine $connection
		if ($Engine -notin $engines) { throw [ArgumentOutOfRangeException] "Engine" }
	}

	process {
		$schemas = $Schema ? ($Schema | ForEach-Object { [Schema]@{ Name = $_ } }) : (Get-MySqlSchema $connection)
		$tables = $schemas | ForEach-Object {
			$schemaObject = $_
			$Table ? ($Table | ForEach-Object { [Table]@{ Name = $_; Schema = $schemaObject.Name } }) : (Get-MySqlTable $connection $schemaObject)
		}

		$tables | ForEach-Object {
			"Processing: $($_.GetQualifiedName($false))"
			Invoke-SqlNonQuery $connection -Command "SET foreign_key_checks = 0" | Out-Null
			Invoke-SqlNonQuery $connection -Command "ALTER TABLE $($_.GetQualifiedName($true)) ENGINE = $Engine" | Out-Null
			Invoke-SqlNonQuery $connection -Command "SET foreign_key_checks = 1" | Out-Null
		}
	}

	clean {
		Close-SqlConnection $connection
	}
}

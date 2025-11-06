using namespace MySqlConnector
using namespace System.Diagnostics.CodeAnalysis
using module ./MySql/Get-Collation.psm1
using module ./MySql/Get-Schema.psm1
using module ./MySql/Get-Table.psm1
using module ./MySql/Invoke-NonQuery.psm1
using module ./MySql/New-Connection.psm1
using module ./MySql/Schema.psm1
using module ./MySql/Table.psm1

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
#>
function Set-MySqlCharset {
	[CmdletBinding()]
	[OutputType([void])]
	[SuppressMessage("PSUseOutputTypeCorrectly", "")]
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

	$connection = $null
	try {
		$connection = New-Connection $Uri -Open
		$collations = Get-Collation $connection
		if ($Collation -notin $collations) { throw [ArgumentOutOfRangeException] "Collation" }

		$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-Schema $connection)
		$tables = foreach ($schemaObject in $schemas) {
			$Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $schemaObject.Name } } : (Get-Table $connection $schemaObject)
		}

		foreach ($tableObject in $tables) {
			"Processing: $($tableObject.QualifiedName($false))"
			[MySqlCommand]::new("SET foreign_key_checks = 0", $connection).ExecuteNonQuery() | Out-Null

			$charset = ($Collation -split "_")[0]
			$command = [MySqlCommand]::new("ALTER TABLE $($tableObject.QualifiedName($true)) CONVERT TO CHARACTER SET $charset COLLATE $Collation", $connection)
			$result = Invoke-NonQuery $command
			if ($result.IsFailure) { Write-Error ($result.Message ? $result.Message : "An error occurred.") }

			[MySqlCommand]::new("SET foreign_key_checks = 1", $connection).ExecuteNonQuery() | Out-Null
		}

		$connection.Close()
	}
	finally {
		${connection}?.Dispose()
	}
}

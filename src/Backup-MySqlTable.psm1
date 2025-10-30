using namespace MySqlConnector
using namespace System.Collections.Generic
using namespace System.IO
using namespace System.Web
using module ./MySql/BackupFormat.psm1
using module ./MySql/Get-Schema.psm1
using module ./MySql/Get-Table.psm1
using module ./MySql/New-Connection.psm1
using module ./MySql/Schema.psm1

<#
.SYNOPSIS
	Backups a set of MariaDB/MySQL tables.
.PARAMETER Uri
	The connection URI.
.PARAMETER Path
	The path to the output directory.
.PARAMETER Format
	The format of the output files.
.PARAMETER Schema
	The schema name.
.PARAMETER Table
	The table name.
#>
function Backup-MySqlTable {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[ValidateNotNull()]
		[uri] $Uri,

		[Parameter(Mandatory, Position = 1)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $Path,

		[ValidateSet("jsonl", "sql")]
		[string] $Format = [BackupFormat]::SqlDump,

		[Parameter()]
		[string[]] $Schema = @(),

		[Parameter()]
		[string[]] $Table = @()
	)

	if ($Format -eq [BackupFormat]::JsonLines) {
		Write-Warning "The ""JSON Lines"" format does not export INVISIBLE columns."
	}

	$connection = $null
	New-Item $Path -Force -ItemType Directory | Out-Null

	try {
		$connection = New-Connection $Uri -Open
		$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-Schema $connection)
		foreach ($schemaObject in $schemas) {
			"Exporting: $($Table.Count -eq 1 ? "$($schemaObject.Name).$($Table[0])" : $schemaObject.Name)"
			if ($Format -eq [BackupFormat]::JsonLines) { Export-JsonLines $schemaObject $Path -Connection $connection -Table $Table }
			else { Export-SqlDump $schemaObject $Path -Table $Table -Uri $Uri }
		}

		$connection.Close()
	}
	finally {
		${connection}?.Dispose()
	}
}

<#
.SYNOPSIS
	Exports the specified schema to a set of JSON Lines files in the specified directory.
.PARAMETER Schema
	The database schema.
.PARAMETER Path
	The path to the output directory.
.PARAMETER Connection
	The database connection.
.PARAMETER Table
	The table name.
#>
function Export-JsonLines {
	[OutputType([void])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[ValidateNotNull()]
		[Schema] $Schema,

		[Parameter(Mandatory, Position = 1)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $Path,

		[Parameter(Mandatory)]
		[ValidateNotNull()]
		[MySqlConnection] $Connection,

		[Parameter()]
		[string[]] $Table = @()
	)

	$tables = $Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $Schema.Name } } : (Get-Table $Connection $Schema)
	foreach ($tableObject in $tables) {
		$command = [MySqlCommand]::new("SELECT * FROM $($tableObject.QualifiedName($true))", $Connection)
		$file = [File]::CreateText((Join-Path $Path "$($tableObject.QualifiedName()).$([BackupFormat]::JsonLines)"))
		$reader = $command.ExecuteReader()
		while ($reader.Read()) {
			$record = @{}
			for ($i = 0; $i -lt $reader.FieldCount; $i++) { $record[$reader.GetName($i)] = $reader.IsDBNull($i) ? $null : $reader.GetValue($i) }
			$file.WriteLine((ConvertTo-Json $record -Compress))
		}

		$reader.Close()
		$file.Close()
	}
}

<#
.SYNOPSIS
	Exports the specified schema to a SQL dump in the specified directory.
.PARAMETER Schema
	The database schema.
.PARAMETER Path
	The path to the output directory.
.PARAMETER Uri
	The connection URI.
.PARAMETER Table
	The table name.
#>
function Export-SqlDump {
	[OutputType([void])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[ValidateNotNull()]
		[Schema] $Schema,

		[Parameter(Mandatory, Position = 1)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $Path,

		[Parameter(Mandatory)]
		[ValidateNotNull()]
		[uri] $Uri,

		[Parameter()]
		[string[]] $Table = @()
	)

	$file = "$($Table.Count -eq 1 ? "$($Schema.Name).$($Table[0])" : $Schema.Name).$([BackupFormat]::SqlDump)"
	$userInfo = ($Uri.UserInfo -split ":").ForEach{ [Uri]::UnescapeDataString($_) }

	$arguments = [List[string]] @(
		"--default-character-set=$([HttpUtility]::ParseQueryString($Uri.Query)["charset"] ?? "utf8mb4")"
		"--host=$($Uri.Host)"
		"--password=$($userInfo[1])"
		"--port=$($Uri.IsDefaultPort ? 3306 : $Uri.Port)"
		"--result-file=$(Join-Path $Path $file)"
		"--user=$($userInfo[0])"
	)

	if ($Uri.Host -notin "::1", "127.0.0.1", "localhost") { $arguments.Add("--compress") }
	$arguments.Add($Schema.Name)
	$arguments.AddRange($Table)

	& mysqldump @arguments
}

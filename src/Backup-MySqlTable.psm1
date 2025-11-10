using namespace MySqlConnector
using namespace System.Collections.Generic
using namespace System.Diagnostics.CodeAnalysis
using namespace System.IO
using namespace System.Web
using module ./MySql/BackupFormat.psm1
using module ./MySql/Get-MySqlSchema.psm1
using module ./MySql/Get-MySqlTable.psm1
using module ./MySql/New-MySqlConnection.psm1
using module ./MySql/Schema.psm1
using module ./MySql/Table.psm1

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
	[SuppressMessage("PSUseOutputTypeCorrectly", "")]
	param (
		[Parameter(Mandatory, Position = 0)]
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

	$connection = New-MySqlConnection $Uri
	New-Item $Path -Force -ItemType Directory | Out-Null

	$schemas = $Schema ? @($Schema.ForEach{ [Schema]@{ Name = $_ } }) : (Get-MySqlSchema $connection)
	foreach ($schemaObject in $schemas) {
		"Exporting: $($Table.Count -eq 1 ? "$($schemaObject.Name).$($Table[0])" : $schemaObject.Name)"
		if ($Format -eq [BackupFormat]::JsonLines) { Export-JsonLine $schemaObject $Path -Connection $connection -Table $Table }
		else { Export-SqlDump $schemaObject $Path -Table $Table -Uri $Uri }
	}

	Close-SqlConnection $connection
}

<#
.SYNOPSIS
	Exports the specified schema to a set of JSON Lines files in the specified directory.
.PARAMETER Schema
	The database schema.
.PARAMETER Path
	The path to the output directory.
.PARAMETER Connection
	The connection to the data source.
.PARAMETER Table
	The table name.
#>
function Export-JsonLine {
	[OutputType([void])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[Schema] $Schema,

		[Parameter(Mandatory, Position = 1)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $Path,

		[Parameter(Mandatory)]
		[MySqlConnection] $Connection,

		[Parameter()]
		[string[]] $Table = @()
	)

	$tables = $Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $Schema.Name } } : (Get-MySqlTable $Connection $Schema)
	foreach ($tableObject in $tables) {
		$file = [File]::CreateText("$Path/$($tableObject.QualifiedName()).$([BackupFormat]::JsonLines)")
		$records = Invoke-SqlQuery $Connection -Command "SELECT * FROM $($tableObject.GetQualifiedName($true))"
		$records.ForEach{ $file.WriteLine((ConvertTo-Json $_ -Compress)) }
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
		[Schema] $Schema,

		[Parameter(Mandatory, Position = 1)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $Path,

		[Parameter(Mandatory)]
		[uri] $Uri,

		[Parameter()]
		[string[]] $Table = @()
	)

	$file = "$($Table.Count -eq 1 ? "$($Schema.Name).$($Table[0])" : $Schema.Name).$([BackupFormat]::SqlDump)"
	$userName, $password = ($Uri.UserInfo -split ":").ForEach{ [Uri]::UnescapeDataString($_) }
	$arguments = [List[string]] @(
		"--default-character-set=$([HttpUtility]::ParseQueryString($Uri.Query)["charset"] ?? "utf8mb4")"
		"--host=$($Uri.Host)"
		"--password=$password"
		"--port=$($Uri.IsDefaultPort ? 3306 : $Uri.Port)"
		"--result-file=$(Join-Path $Path $file)"
		"--user=$userName"
	)

	if ($Uri.Host -notin "::1", "127.0.0.1", "localhost") { $arguments.Add("--compress") }
	$arguments.Add($Schema.Name)
	$arguments.AddRange($Table)
	mysqldump @arguments
}

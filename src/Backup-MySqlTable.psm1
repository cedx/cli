using namespace Belin.Cli.MySql
using namespace MySqlConnector
using namespace System.Collections.Generic
using namespace System.Diagnostics.CodeAnalysis
using namespace System.IO
using namespace System.Web

<#
.SYNOPSIS
	Backups a set of MariaDB/MySQL tables.
.OUTPUTS
	The log messages.
#>
function Backup-MySqlTable {
	[CmdletBinding()]
	[OutputType([string])]
	param (
		# The connection URI.
		[Parameter(Mandatory, Position = 0)]
		[uri] $Uri,

		# The path to the output directory.
		[Parameter(Mandatory, Position = 1)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $Path,

		# The format of the output files.
		[ValidateSet("jsonl", "sql")]
		[string] $Format = [BackupFormat]::SqlDump,

		# The schema name.
		[Parameter()]
		[string[]] $Schema = @(),

		# The table name.
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
#>
function Export-JsonLine {
	[OutputType([void])]
	param (
		# The database schema.
		[Parameter(Mandatory, Position = 0)]
		[Schema] $Schema,

		# The path to the output directory.
		[Parameter(Mandatory, Position = 1)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $Path,

		# The connection to the data source.
		[Parameter(Mandatory)]
		[MySqlConnection] $Connection,

		# The table name.
		[Parameter()]
		[string[]] $Table = @()
	)

	$tables = $Table ? $Table.ForEach{ [Table]@{ Name = $_; Schema = $Schema.Name } } : (Get-MySqlTable $Connection $Schema)
	foreach ($tableObject in $tables) {
		$file = [File]::CreateText("$Path/$($tableObject.QualifiedName).$([BackupFormat]::JsonLines)")
		$records = Invoke-SqlQuery $Connection -Command "SELECT * FROM $($tableObject.GetQualifiedName($true))" -Stream
		$records.ForEach{ $file.WriteLine((ConvertTo-Json $_ -Compress)) }
		$file.Close()
	}
}

<#
.SYNOPSIS
	Exports the specified schema to a SQL dump in the specified directory.
#>
function Export-SqlDump {
	[OutputType([void])]
	param (
		# The database schema.
		[Parameter(Mandatory, Position = 0)]
		[Schema] $Schema,

		# The path to the output directory.
		[Parameter(Mandatory, Position = 1)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $Path,

		# The connection URI.
		[Parameter(Mandatory)]
		[uri] $Uri,

		# The table name.
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

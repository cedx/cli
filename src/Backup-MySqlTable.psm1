using namespace Belin.Cli.MySql
using namespace System.Collections.Generic
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

		# The schema name.
		[Parameter()]
		[string[]] $Schema = @(),

		# The table name.
		[Parameter()]
		[string[]] $Table = @()
	)

	begin {
		$connection = New-MySqlConnection $Uri
		New-Item $Path -Force -ItemType Directory | Out-Null
	}

	process {
		$schemas = $Schema ? ($Schema | ForEach-Object { [Schema]@{ Name = $_ } }) : (Get-MySqlSchema $connection)
		$schemas | ForEach-Object {
			"Exporting: $($Table.Count -eq 1 ? "$($_.Name).$($Table[0])" : $_.Name)"
			Export-SqlDump $_ -Path $Path -Table $Table -Uri $Uri
		}
	}

	clean {
		Close-SqlConnection $connection
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

	$file = "$($Table.Count -eq 1 ? "$($Schema.Name).$($Table[0])" : $Schema.Name).sql"
	$userName, $password = $Uri.UserInfo -split ":" | ForEach-Object { [Uri]::UnescapeDataString($_) }
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

using namespace System.Collections.Generic
using namespace System.Web

<#
.SYNOPSIS
	Restores a set of MariaDB/MySQL tables.
.INPUTS
	A string that contains a path, but not a literal path.
.OUTPUTS
	The log messages.
#>
function Restore-MySqlTable {
	[CmdletBinding(DefaultParameterSetName = "Path")]
	[OutputType([string])]
	param (
		# The connection URI.
		[Parameter(Mandatory, Position = 0)]
		[uri] $Uri,

		# Specifies the path to an SQL dump.
		[Parameter(Mandatory, ParameterSetName = "Path", Position = 1, ValueFromPipeline)]
		[SupportsWildcards()]
		[string[]] $Path,

		# Specifies the literal path to an SQL dump.
		[Parameter(Mandatory, ParameterSetName = "LiteralPath")]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified literal path is invalid.")]
		[string[]] $LiteralPath,

		# A pattern used to filter the list of files to be processed.
		[Parameter()]
		[string] $Filter = "*.sql",

		# Value indicating whether to process the input path recursively.
		[Parameter()]
		[switch] $Recurse
	)

	process {
		$parameters = @{ File = $true; Recurse = $Recurse }
		if ($Filter) { $parameters.Filter = $Filter }
		$files = $PSCmdlet.ParameterSetName -eq "LiteralPath" ? (Get-ChildItem -LiteralPath $LiteralPath @parameters) : (Get-ChildItem $Path @parameters)

		$files | ForEach-Object {
			"Importing: $($_.BaseName)"
			$userName, $password = $Uri.UserInfo -split ":" | ForEach-Object { [Uri]::UnescapeDataString($_) }
			$arguments = [List[string]] @(
				"--default-character-set=$([HttpUtility]::ParseQueryString($Uri.Query)["charset"] ?? "utf8mb4")"
				"--execute=USE $($_.BaseName); SOURCE $($_.FullName -replace "\", "/");"
				"--host=$($Uri.Host)"
				"--password=$password"
				"--port=$($Uri.IsDefaultPort ? 3306 : $Uri.Port)"
				"--user=$userName"
			)

			if ($Uri.Host -notin "::1", "127.0.0.1", "localhost") { $arguments.Add("--compress") }
			mysql @arguments
		}
	}
}

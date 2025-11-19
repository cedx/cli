using namespace System.Collections.Generic
using namespace System.Diagnostics.CodeAnalysis
using namespace System.Web

<#
.SYNOPSIS
	Restores a set of MariaDB/MySQL tables.
.PARAMETER Path
	Specifies the path to an SQL dump.
.PARAMETER LiteralPath
	Specifies the literal path to an SQL dump.
.PARAMETER Recurse
	Value indicating whether to process the input path recursively.
.INPUTS
	A string that contains a path, but not a literal path.
#>
function Restore-MySqlTable {
	[CmdletBinding(DefaultParameterSetName = "Path")]
	[OutputType([void])]
	[SuppressMessage("PSUseOutputTypeCorrectly", "")]
	param (
		[Parameter(Mandatory, Position = 0)]
		[uri] $Uri,

		[Parameter(Mandatory, ParameterSetName = "Path", Position = 1, ValueFromPipeline)]
		[SupportsWildcards()]
		[string[]] $Path,

		[Parameter(Mandatory, ParameterSetName = "LiteralPath")]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified literal path is invalid.")]
		[string[]] $LiteralPath,

		[Parameter()]
		[switch] $Recurse
	)

	process {
		$parameters = @{ File = $true; Recurse = $Recurse }
		$files = $PSCmdlet.ParameterSetName -eq "LiteralPath" ? (Get-ChildItem -LiteralPath $LiteralPath @parameters) : (Get-ChildItem $Path @parameters)

		foreach ($file in $files) {
			"Importing: $($file.BaseName)"
			$userName, $password = ($Uri.UserInfo -split ":").ForEach{ [Uri]::UnescapeDataString($_) }
			$arguments = [List[string]] @(
				"--default-character-set=$([HttpUtility]::ParseQueryString($Uri.Query)["charset"] ?? "utf8mb4")"
				"--execute=USE $($file.BaseName); SOURCE $($file.FullName -replace "\\", "/");"
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

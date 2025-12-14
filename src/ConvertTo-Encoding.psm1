using namespace System.Diagnostics.CodeAnalysis
using namespace System.IO
using namespace System.Text

<#
.SYNOPSIS
	The list of binary file extensions.
#>
$BinaryExtensions = @()

<#
.SYNOPSIS
	The list of text file extensions.
#>
$TextExtensions = @()

<#
.SYNOPSIS
	Converts the encoding of input files.
.PARAMETER Path
	The path to the files to convert.
.PARAMETER LiteralPath
	The path to the files to convert.
.PARAMETER To
	The destination encoding.
.PARAMETER Exclude
	The list of folders to exclude from the processing.
.PARAMETER Recurse
	Value indicating whether to process the input path recursively.
.INPUTS
	A string that contains a path, but not a literal path.
.OUTPUTS
	The log messages.
#>
function ConvertTo-Encoding {
	[CmdletBinding(DefaultParameterSetName = "Path")]
	[OutputType([string])]
	param (
		[Parameter(Mandatory, ParameterSetName = "Path", Position = 0, ValueFromPipeline)]
		[SupportsWildcards()]
		[string[]] $Path,

		[Parameter(Mandatory, ParameterSetName = "LiteralPath")]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified literal path is invalid.")]
		[string[]] $LiteralPath,

		[Parameter(Mandatory)]
		[ValidateSet("Latin1", "UTF-8")]
		[string] $Encoding,

		[ValidateNotNull()]
		[string[]] $Exclude = @(".git", "node_modules", "ps_modules", "vendor"),

		[Parameter()]
		[switch] $Recurse
	)

	begin {
		if (-not $Script:BinaryExtensions) { $Script:BinaryExtensions = Get-Content "$PSScriptRoot/../res/BinaryExtensions.json" | ConvertFrom-Json }
		if (-not $Script:TextExtensions) { $Script:TextExtensions = Get-Content "$PSScriptRoot/../res/TextExtensions.json" | ConvertFrom-Json }
	}

	process {
		$sourceEncoding = [Encoding]::GetEncoding($Encoding -eq "Latin1" ? "UTF-8" : "Latin1")
		$destinationEncoding = [Encoding]::GetEncoding($Encoding)

		$parameters = @{ File = $true; Recurse = $Recurse }
		$files = $PSCmdlet.ParameterSetName -eq "LiteralPath" ? (Get-ChildItem -LiteralPath $LiteralPath @parameters) : (Get-ChildItem $Path @parameters)

		foreach ($file in $files) {
			if (Test-IsExcluded $file -Exclude $Exclude) { continue }

			$extension = Split-Path $file.Name -Extension
			$isBinary = $extension -and ($extension.Substring(1) -in $Script:BinaryExtensions)
			if ($isBinary) { continue }

			$bytes = Get-Content $file.FullName -AsByteStream
			if (-not $bytes) { continue }

			$isText = $extension -and ($extension.Substring(1) -in $Script:TextExtensions)
			if ((-not $isText) -and ([Array]::IndexOf[byte]($bytes, 0, 0, [Math]::Min($bytes.Count, 8000)) -gt 0)) { continue }

			"Converting: $file"
			Set-Content $file.FullName ([Encoding]::Convert($sourceEncoding, $destinationEncoding, $bytes)) -AsByteStream
		}
	}
}

<#
.SYNOPSIS
	Checks if the specified file should be excluded from the processing.
.PARAMETER File
	The file to be checked.
.PARAMETER Exclude
	The list of folders to exclude from the processing.
.OUTPUTS
	`$true` if the specified file should be excluded from the processing, otherwise `$false`.
#>
function Test-IsExcluded {
	[OutputType([bool])]
	param (
		[Parameter(Mandatory, Position = 0, ValueFromPipeline)]
		[FileInfo] $File,

		[ValidateNotNull()]
		[string[]] $Exclude = @(".git", "node_modules", "ps_modules", "vendor")
	)

	process {
		$directory = $file.Directory
		while ($directory) {
			if ($directory.Name -in $Exclude) { return $true }
			$directory = $directory.Parent
		}

		$false
	}
}

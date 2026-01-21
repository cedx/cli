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
.INPUTS
	A string that contains a path, but not a literal path.
.OUTPUTS
	The log messages.
#>
function ConvertTo-Encoding {
	[CmdletBinding(DefaultParameterSetName = "Path")]
	[OutputType([string])]
	param (
		# The path to the files to convert.
		[Parameter(Mandatory, ParameterSetName = "Path", Position = 0, ValueFromPipeline)]
		[SupportsWildcards()]
		[string[]] $Path,

		# The literal path to the files to convert.
		[Parameter(Mandatory, ParameterSetName = "LiteralPath")]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified literal path is invalid.")]
		[string[]] $LiteralPath,

		# The destination encoding.
		[Parameter(Mandatory)]
		[ValidateSet("Latin1", "UTF-8")]
		[string] $Encoding,

		# The list of folders to exclude from the processing.
		[ValidateNotNull()]
		[string[]] $Exclude = @(".git", "node_modules", "vendor"),

		# A pattern used to filter the list of files to be processed.
		[Parameter()]
		[string] $Filter = "",

		# Value indicating whether to process the input path recursively.
		[Parameter()]
		[switch] $Recurse
	)

	begin {
		$resources = Join-Path $PSScriptRoot ../res
		if (-not $Script:BinaryExtensions) { $Script:BinaryExtensions = Get-Content "$resources/BinaryExtensions.json" | ConvertFrom-Json }
		if (-not $Script:TextExtensions) { $Script:TextExtensions = Get-Content "$resources/TextExtensions.json" | ConvertFrom-Json }
	}

	process {
		$sourceEncoding = [Encoding]::GetEncoding($Encoding -eq "Latin1" ? "UTF-8" : "Latin1")
		$destinationEncoding = [Encoding]::GetEncoding($Encoding)

		$parameters = @{ File = $true; Recurse = $Recurse }
		if ($Filter) { $parameters.Filter = $Filter }
		$files = $PSCmdlet.ParameterSetName -eq "LiteralPath" ? (Get-ChildItem -LiteralPath $LiteralPath @parameters) : (Get-ChildItem $Path @parameters)

		$files | Where-Object { -not (Test-IsExcluded -Exclude $Exclude) } | ForEach-Object {
			$extension = Split-Path $_.Name -Extension
			$isBinary = $extension -and ($extension.Substring(1) -in $Script:BinaryExtensions)
			if ($isBinary) { return }

			$bytes = Get-Content $_.FullName -AsByteStream
			if (-not $bytes) { return }

			$isText = $extension -and ($extension.Substring(1) -in $Script:TextExtensions)
			if ((-not $isText) -and ([Array]::IndexOf[byte]($bytes, 0, 0, [Math]::Min($bytes.Count, 8000)) -gt 0)) { return }

			"Converting: $_"
			Set-Content $_.FullName ([Encoding]::Convert($sourceEncoding, $destinationEncoding, $bytes)) -AsByteStream
		}
	}
}

<#
.SYNOPSIS
	Checks if the specified file should be excluded from the processing.
.OUTPUTS
	`$true` if the specified file should be excluded from the processing, otherwise `$false`.
#>
function Test-IsExcluded {
	[OutputType([bool])]
	param (
		# The file to be checked.
		[Parameter(Mandatory, Position = 0, ValueFromPipeline)]
		[FileInfo] $File,

		# The list of folders to exclude from the processing.
		[ValidateNotNull()]
		[string[]] $Exclude = @(".git", "node_modules", "vendor")
	)

	process {
		$directory = $File.Directory
		while ($directory) {
			if ($directory.Name -in $Exclude) { return $true }
			$directory = $directory.Parent
		}

		$false
	}
}

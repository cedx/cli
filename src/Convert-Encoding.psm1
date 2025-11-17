using namespace System.IO
using namespace System.Text

<#
.SYNOPSIS
	The list of binary file extensions.
#>
$BinaryExtensions = @()

<#
.SYNOPSIS
	The list of folders to exclude from the processing.
#>
$ExludedFolders = ".git", "node_modules", "ps_modules", "vendor"

<#
.SYNOPSIS
	The list of text file extensions.
#>
$TextExtensions = @()

<#
.SYNOPSIS
	Converts the encoding of input files.
.PARAMETER Path
	The path to the file to convert.
.PARAMETER LiteralPath
	The path to the file to convert.
.PARAMETER From
	The source encoding.
.PARAMETER To
	The destination encoding.
.PARAMETER Recurse
	Value indicating whether to process the input path recursively.
.INPUTS
	A string that contains a path, but not a literal path.
#>
function Convert-Encoding {
	[CmdletBinding(DefaultParameterSetName = "Path")]
	[OutputType([void])]
	param (
		[Parameter(Mandatory, ParameterSetName = "Path", Position = 0, ValueFromPipeline)]
		[SupportsWildcards()]
		[string[]] $Path,

		[Parameter(Mandatory, ParameterSetName = "LiteralPath")]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified literal path is invalid.")]
		[string[]] $LiteralPath,

		[ValidateSet("Latin1", "UTF-8")]
		[string] $From = [Encoding]::Latin1.WebName,

		[ValidateSet("Latin1", "UTF-8")]
		[string] $To = [Encoding]::UTF8.WebName,

		[Parameter()]
		[switch] $Recurse
	)

	begin {
		if (-not $Script:BinaryExtensions) { $Script:BinaryExtensions = Get-Content "$PSScriptRoot/../res/BinaryExtensions.json" | ConvertFrom-Json }
		if (-not $Script:TextExtensions) { $Script:TextExtensions = Get-Content "$PSScriptRoot/../res/TextExtensions.json" | ConvertFrom-Json }
	}

	process {
		$srcEncoding = [Encoding]::GetEncoding($From)
		$dstEncoding = [Encoding]::GetEncoding($to)

		$files = $PSCmdlet.ParameterSetName -eq "LiteralPath" ? (Get-ChildItem -LiteralPath $LiteralPath -Recurse:$Recurse) : (Get-ChildItem $Path -Recurse:$Recurse)
		foreach ($file in $files) {
		}
	}
}

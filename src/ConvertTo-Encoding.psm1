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
	The path to the file or directory to process.
.PARAMETER LiteralPath
	The path to the file or directory to process.
.PARAMETER From
	The input encoding.
.PARAMETER To
	The output encoding.
.PARAMETER Recurse
	Value indicating whether to process the input directory recursively.
.INPUTS
	A string that contains a path.
.OUTPUTS
	Returns a string representing the content.
#>
function ConvertTo-Encoding {
	[CmdletBinding(DefaultParameterSetName = "Path")]
	[OutputType([void])]
	param (
		[Parameter(Mandatory, ParameterSetName = "Path", Position = 0, ValueFromPipeline)]
		[ValidateNotNullOrWhiteSpace()]
		[string[]] $Path,

		[Parameter(Mandatory, ParameterSetName = "LiteralPath")]
		[ValidateNotNullOrWhiteSpace()]
		[string[]] $LiteralPath,

		[Parameter(Mandatory, ParameterSetName = "InputObject", ValueFromPipeline)]
		[ValidateNotNull()]
		[FileInfo] $InputObject,

		[ValidateSet("Latin1", "UTF8")]
		[string] $From = "Latin1",

		[ValidateSet("Latin1", "UTF8")]
		[string] $To = "UTF8",

		[Parameter()]
		[switch] $Recurse
	)

	begin {
		$exludedFolders = ".git", "node_modules", "ps_modules", "vendor"
		$binaryExtensions = Get-Content "$PSScriptRoot/../res/BinaryExtensions.json" | ConvertFrom-Json
		$textExtensions = Get-Content "$PSScriptRoot/../res/TextExtensions.json" | ConvertFrom-Json
	}

	process {
		$from = [Encoding]::GetEncoding($From)
		$to = [Encoding]::GetEncoding($to)
		$items =

		$resolvedPaths = $PSCmdlet.ParameterSetName -eq "LiteralPath" ? (Resolve-Path -LiteralPath $LiteralPath) : (Resolve-Path $Path)
		foreach ($path in $resolvedPaths) {
			$item = G
		}
	}
}

<#
.SYNOPSIS
	Converts the encoding of the specified file to Latin1 format.
#>
function ConvertTo-Latin1Encoding {

}


<#
.SYNOPSIS
	Converts the encoding of the specified file to UTF-8 format.
#>
function ConvertTo-Utf8Encoding {

}

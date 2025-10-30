<#
.SYNOPSIS
	Extracts the specified TAR archive into a given directory.
.PARAMETER Path
	The path to the input TAR archive.
.PARAMETER DestinationPath
	The path to the output directory.
.PARAMETER Skip
	The number of leading directory components to remove from file names on extraction.
#>
function Expand-TarArchive {
	[OutputType([void])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[ValidateScript({ Test-Path $_ -PathType Leaf }, ErrorMessage = "The specified input file does not exist.")]
		[string] $Path,

		[Parameter(Mandatory, Position = 1)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $DestinationPath,

		[ValidateRange("NonNegative")]
		[int] $Skip = 0
	)

	New-Item $DestinationPath -Force -ItemType Directory | Out-Null
	tar --directory $DestinationPath --extract --file $Path --strip-components $Skip
}

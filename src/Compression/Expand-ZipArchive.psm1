using namespace System.IO.Compression

<#
.SYNOPSIS
	Extracts the specified ZIP archive into a given directory.
.PARAMETER Path
	The path to the input ZIP archive.
.PARAMETER DestinationPath
	The path to the output directory.
.PARAMETER Skip
	The number of leading directory components to remove from file names on extraction.
#>
function Expand-ZipArchive {
	[OutputType([void])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[ValidateScript({ Test-Path $_ }, ErrorMessage = "The input file does not exist.")]
		[string] $Path,

		[Parameter(Mandatory, Position = 1)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The output path is invalid.")]
		[string] $DestinationPath,

		[ValidateRange("NonNegative")]
		[int] $Skip = 0
	)

	if ($Skip -eq 0) { Expand-Archive $Path $DestinationPath -Force }
	else {
		$archive = $null
		try {
			$archive = [ZipFile]::OpenRead($Path)
			foreach ($entry in $archive.Entries) {
				$components = $entry.FullName -split "/"
				$newPath = $components[$Skip..($components.Count - 1)] -join "/"
				if (-not $newPath) { $newPath = "/" }

				$fullPath = Join-Path $DestinationPath $newPath
				if ($newPath[-1] -eq "/") { New-Item $fullPath -Force -ItemType Directory | Out-Null }
				else { [ZipFileExtensions]::ExtractToFile($entry, $fullPath, $true) }
			}
		}
		finally {
			${archive}?.Dispose()
		}
	}
}

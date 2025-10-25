using namespace System.IO
using module ./Compression/Expand-Archive.psm1

<#
.SYNOPSIS
	Downloads and installs the latest Node.js release.
.PARAMETER Path
	The path to the output directory.
#>
function Install-Node {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Position = 0)]
		[ValidateScript({ Test-Path $_ -IsValid })]
		[string] $Path = $IsWindows ? "C:\Program Files\Node.js" : "/usr/local"
	)

	end {
		$operatingSystem, $fileExtension = switch ($true) {
			($IsMacOS) { "darwin", "tar.gz"; break }
			($IsWindows) { "win", "zip"; break }
			default { "linux", "tar.xz" }
		}

		"Fetching the list of Node.js releases..."
		$response = Invoke-RestMethod "https://nodejs.org/dist/index.json"
		$version = [version] $response[0].version.Substring(1)

		$fileName = "node-v$version-$operatingSystem-x64.$fileExtension"
		"Downloading file ""$fileName""..."
		$outputFile = New-TemporaryFile
		Invoke-WebRequest "https://nodejs.org/dist/v$version/$fileName" -OutFile $outputFile

		"Extracting file ""$fileName"" into directory ""$Path""..."
		if ($fileExtension -eq "zip") { Expand-ZipArchive $outputFile $Path -Skip 1 }
		else { Expand-TarArchive $outputFile $Path -Skip 1 }

		$executable = $IsWindows ? "node.exe" : "bin/node"
		& $Path/$executable --version
	}
}

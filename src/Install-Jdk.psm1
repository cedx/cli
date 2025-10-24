using namespace System.IO
using module ./Compression/Expand-Archive.psm1

<#
.SYNOPSIS
	Downloads and installs the latest OpenJDK release.
.PARAMETER Path
	The path to the output directory.
.PARAMETER Version
	The major version of the Java development kit.
#>
function Install-Jdk {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Position = 0)]
		[ValidateScript({ Test-Path $_ -IsValid })]
		[string] $Path = $IsWindows ? "C:\Program Files\OpenJDK" : "/opt/openjdk",

		[ValidateSet(11, 17, 21, 25)]
		[int] $Version = 25
	)

	end {
		$operatingSystem, $fileExtension = switch ($true) {
			($IsMacOS) { "macOS", "tar.gz"; break }
			($IsWindows) { "windows", "zip"; break }
			default { "linux", "tar.gz" }
		}

		$fileName = "microsoft-jdk-$Version-$operatingSystem-x64.$fileExtension"
		"Downloading file ""$fileName""..."
		$outputFile = New-TemporaryFile
		Invoke-WebRequest "https://aka.ms/download-jdk/$fileName" -OutFile $outputFile

		"Extracting file ""$fileName"" into directory ""$Path""..."
		if ($fileExtension -eq "zip") { Expand-ZipArchive $outputFile $Path -Skip 1 }
		else { Expand-TarArchive $outputFile $Path -Skip 1 }

		$executable = $IsWindows ? "java.exe" : "java"
		& $Path/bin/$executable --version
	}
}

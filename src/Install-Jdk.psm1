using namespace System.IO
using module ./Compression/Expand-Archive.psm1

<#
.SYNOPSIS
	Downloads and installs the latest OpenJDK release.
.PARAMETER Path
	The path to the output directory.
.PARAMETER Java
	The major version of the Java development kit.
#>
function Install-Jdk {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Position = 0)]
		[ValidateScript({ Test-Path $_ -IsValid })]
		[string] $Path = $IsWindows ? "C:\Program Files\OpenJDK" : "/opt/openjdk",

		[ValidateSet(8, 11, 17, 21, 25)]
		[int] $Java = 25
	)

	end {
		$operatingSystem, $fileExtension = switch ($true) {
			($IsMacOS) { "macOS", "tar.gz"; break }
			($IsWindows) { "windows", "zip"; break }
			default { "linux", "tar.gz" }
		}

		$fileName = "microsoft-jdk-$Java-$operatingSystem-x64.$fileExtension"
		Write-Output "Downloading file ""$fileName""..."
		$outputFile = New-TemporaryFile
		Invoke-WebRequest "https://aka.ms/download-jdk/$fileName" -OutFile $outputFile

		Write-Output "Extracting file ""$fileName"" into directory ""$Path""..."
		if ($fileExtension -eq "zip") { Expand-ZipArchive $outputFile $Path -Skip 1 }
		else { Expand-TarAchive $outputFile $Path -Skip 1 }

		& $Path/bin/java --version
	}
}

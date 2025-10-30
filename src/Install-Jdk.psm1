using module ./Compression/Expand-TarArchive.psm1
using module ./Compression/Expand-ZipArchive.psm1
using module ./Test-Privilege.psm1

<#
.SYNOPSIS
	Downloads and installs the latest OpenJDK release.
.PARAMETER Path
	The path to the output directory.
.PARAMETER Version
	The major version of the Java development kit.
.OUTPUTS
	The output from the `java --version` command.
#>
function Install-Jdk {
	[CmdletBinding()]
	[OutputType([string])]
	param (
		[Parameter(Position = 0)]
		[ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The specified output path is invalid.")]
		[string] $Path = $IsWindows ? "C:\Program Files\OpenJDK" : "/opt/openjdk",

		[ValidateSet(11, 17, 21, 25)]
		[int] $Version = 25
	)

	if (-not (Test-Privilege $Path)) {
		throw [InvalidOperationException] "You must run this command in an elevated prompt."
	}

	$platform, $extension = switch ($true) {
		($IsMacOS) { "macOS", "tar.gz"; break }
		($IsWindows) { "windows", "zip"; break }
		default { "linux", "tar.gz" }
	}

	$file = "microsoft-jdk-$Version-$platform-x64.$extension"
	"Downloading file ""$file""..."
	$outputFile = New-TemporaryFile
	Invoke-WebRequest "https://aka.ms/download-jdk/$file" -OutFile $outputFile

	"Extracting file ""$file"" into directory ""$Path""..."
	if ($extension -eq "zip") { Expand-ZipArchive $outputFile $Path -Skip 1 }
	else { Expand-TarArchive $outputFile $Path -Skip 1 }

	$executable = $IsWindows ? "java.exe" : "java"
	& $Path/bin/$executable --version
}

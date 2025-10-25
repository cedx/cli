using namespace System.Diagnostics
using namespace System.IO
using namespace System.ServiceProcess
using module ./Compression/Expand-Archive.psm1

<#
.SYNOPSIS
	Downloads and installs the latest PHP release.
.PARAMETER Path
	The path to the output directory.
.PARAMETER RegisterEventSource
	Value indicating whether to register the PHP interpreter with the event log.
#>
function Install-Php {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Position = 0)]
		[ValidateScript({ Test-Path $_ -IsValid })]
		[string] $Path = "C:\Program Files\PHP",

		[Parameter()]
		[switch] $RegisterEventSource
	)

	end {
		if (-not $IsWindows) { throw [PlatformNotSupportedException] "This command only supports the Windows platform." }

		"Fetching the list of PHP releases..."
		$response = Invoke-RestMethod "https://www.php.net/releases/?json"
		$property = ($response | Get-Member -MemberType NoteProperty | Sort-Object Name -Bottom 1).Name
		$version = [version] $response.$property.version

		$fileName = "php-$version-nts-Win32-$($version -ge [version] "8.4.0" ? "vs17" : "vs16")-x64.zip"
		"Downloading file ""$fileName""..."
		$outputFile = New-TemporaryFile
		Invoke-WebRequest "https://windows.php.net/downloads/releases/$fileName" -OutFile $outputFile

		"Stopping the IIS web server..."
		Stop-Service W3SVC

		"Extracting file ""$fileName"" into directory ""$Path""..."
		Expand-Archive $outputFile $Path -Force

		"Starting the IIS web server..."
		Start-Service W3SVC

		if ($RegisterEventSource) {
			"Registering the PHP interpreter with the event log..."
			$key = "HKLM:\SYSTEM\CurrentControlSet\Services\EventLog\Application\PHP-$version"
			New-Item $key -Force | Out-Null
			New-ItemProperty $key EventMessageFile -Force -PropertyType String -Value (Join-Path $Path "php$($version.Major).dll" -Resolve) | Out-Null
			New-ItemProperty $key TypesSupported -Force -PropertyType DWord -Value 7 | Out-Null
		}

		& $Path/php.exe --version
	}
}

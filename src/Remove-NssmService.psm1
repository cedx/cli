using module ./Nssm/WebApplication.psm1
using module ./Test-Privilege.psm1

<#
.SYNOPSIS
	Unregisters a Windows service based on [NSSM](https://nssm.cc).
.PARAMETER Path
	The path to the root directory of the web application.
#>
function Remove-NssmService {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Position = 0)]
		[ValidateScript({ Test-Path $_ -PathType Container }, ErrorMessage = "The specified directory does not exist.")]
		[string] $Path = $PWD
	)

	if (-not $IsWindows) { throw [PlatformNotSupportedException] "This command only supports the Windows platform." }
	if (-not (Test-Privilege)) { throw [UnauthorizedAccessException] "You must run this command in an elevated prompt." }

	$application = [WebApplication]::ReadFromDirectory($Path)
	if (-not $application) { throw [EntryPointNotFoundException] "Unable to locate the application configuration file." }

	Stop-Service $application.Id
	nssm remove $application.Id confirm
}

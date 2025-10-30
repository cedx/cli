using module ./Nssm/WebApplication.psm1

<#
.SYNOPSIS
	Unregisters a Windows service based on [NSSM](https://nssm.cc).
.PARAMETER Path
	The path to the root directory of the .NET or Node.js application.
#>
function Remove-NssmService {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Position = 0)]
		[ValidateScript({ Test-Path $_ -PathType Container }, ErrorMessage = "The specified directory does not exist.")]
		[string] $Path
	)

	if (-not (Test-Privilege)) {
		throw [UnauthorizedAccessException] "You must run this command in an elevated prompt."
	}

	$application = [WebApplication]::ReadFromDirectory($Path)
	if ($null -eq $application) { throw [EntryPointNotFoundException] "Unable to locate the application configuration file." }

	Stop-Service $application.Id
	nssm remove $application.Id confirm
}

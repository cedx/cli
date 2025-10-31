using namespace System.Diagnostics.CodeAnalysis
using module ./Nssm/WebApplication.psm1
using module ./Test-Privilege.psm1

<#
.SYNOPSIS
	Registers a Windows service based on [NSSM](https://nssm.cc).
.PARAMETER Path
	The path to the root directory of the web application.
.PARAMETER Start
	Value indicating whether to start the service after its registration.
#>
function New-NssmService {
	[CmdletBinding()]
	[OutputType([void])]
	[SuppressMessage("PSUseShouldProcessForStateChangingFunctions", "")]
	param (
		[Parameter(Position = 0)]
		[ValidateScript({ Test-Path $_ -PathType Container }, ErrorMessage = "The specified directory does not exist.")]
		[string] $Path = $PWD,

		[Parameter()]
		[switch] $Start
	)

	if (-not $IsWindows) { throw [PlatformNotSupportedException] "This command only supports the Windows platform." }
	if (-not (Test-Privilege)) { throw [UnauthorizedAccessException] "You must run this command in an elevated prompt." }

	$application = [WebApplication]::ReadFromDirectory($Path)
	if (-not $application) { throw [EntryPointNotFoundException] "Unable to locate the application configuration file." }
	if ($application.Type -eq [WebApplicationType]::Unknown) { throw [NotSupportedException] "The application type could not be determined." }

	if (-not $application.Environment) { $application.Environment = "Production" }
	nssm install $application.Id $application.Program().Path $application.EntryPoint()

	$properties = @{
		AppDirectory = $application.Path
		AppEnvironmentExtra = "$($application.EnvironmentVariable())=$($application.Environment)"
		AppNoConsole = "1"
		AppStderr = Join-Path $application.Path "var/Error.log"
		AppStdout = Join-Path $application.Path "var/Output.log"
		Description = $application.Description
		DisplayName = $application.Name
		Start = "SERVICE_AUTO_START"
	}

	foreach ($key in $properties.Keys) {
		nssm set $application.Id $key $properties.$key
	}

	if ($Start) { Start-Service $application.Id }
}

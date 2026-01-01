using namespace Belin.Cli.Nssm
using namespace Belin.Cli.Validation
using namespace System.Diagnostics.CodeAnalysis

<#
.SYNOPSIS
	Registers a Windows service based on [NSSM](https://nssm.cc).
.OUTPUTS
	The log messages.
#>
function New-NssmService {
	[CmdletBinding()]
	[OutputType([string])]
	[SuppressMessage("PSUseShouldProcessForStateChangingFunctions", "")]
	param (
		# The path to the root directory of the web application.
		[Parameter(Position = 0, ValueFromPipeline)]
		[ValidateScript({ Test-Path $_ -PathType Container }, ErrorMessage = "The specified directory does not exist.")]
		[string] $Path = $PWD,

		# Value indicating whether to start the service after its registration.
		[Parameter()]
		[switch] $Start
	)

	begin {
		if (-not $IsWindows) { throw [PlatformNotSupportedException] "This command only supports the Windows platform." }
		if (-not (Test-Privilege)) { throw [UnauthorizedAccessException] "You must run this command in an elevated prompt." }
	}

	process {
		$application = switch ($true) {
			((Test-Path "$Path/src/Server/*.cs") -or (Test-Path "$Path/src/*.cs")) { [DotNetApplication]::new($Path); break }
			((Test-Path "$Path/src/Server/*.ts") -or (Test-Path "$Path/src/*.ts")) { [NodeApplication]::new($Path); break }
			default { throw [NotSupportedException] "The application type could not be determined." }
		}

		if (Get-Service $application.Id -ErrorAction Ignore) {
			throw [InvalidOperationException] "The service ""$($application.Id)"" already exists."
		}

		$properties = [ordered]@{
			AppDirectory = $application.Path
			AppEnvironmentExtra = "$($application.EnvironmentVariable())=$($application.Environment)"
			AppNoConsole = "1"
			AppStderr = Join-Path $application.Path "var/Error.log"
			AppStdout = Join-Path $application.Path "var/Output.log"
			Description = $application.Description
			DisplayName = $application.Name
			Start = "SERVICE_AUTO_START"
		}

		nssm install $application.Id $application.Program().Path $application.EntryPoint() | Out-Null
		foreach ($key in $properties.Keys) { nssm set $application.Id $key $properties.$key | Out-Null }
		if ($Start) { Start-Service $application.Id }

		$created = $Start ? "started" : "created"
		"The service ""$($application.Id)"" has been successfully $created."
	}
}

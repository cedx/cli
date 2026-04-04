using namespace Belin.Cli.Nssm
using namespace System.Diagnostics.CodeAnalysis
using namespace System.Management.Automation
using module ../Security/Test-Privilege.psm1
using module ./Get-NssmPath.psm1

<#
.SYNOPSIS
	Registers a Windows service based on [NSSM](https://nssm.cc).
.INPUTS
	The path to the root directory of the web application.
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

		# The account used by the service as the logon account.
		[Credential()]
		[pscredential] $Credential,

		# Value indicating whether to start the service after its registration.
		[switch] $Start
	)

	begin {
		if (-not $IsWindows) { throw [PlatformNotSupportedException] "This command only supports the Windows platform." }
		if (-not (Test-Privilege)) { throw [UnauthorizedAccessException] "You must run this command in an elevated prompt." }
	}

	process {
		$application = switch ($true) {
			((Test-Path "$Path/src/Server/*.cs") -or (Test-Path "$Path/src/*.cs")) { [DotNetApplication] $Path; break }
			((Test-Path "$Path/src/Server/*.ts") -or (Test-Path "$Path/src/*.ts")) { [NodeApplication] $Path; break }
			default { throw [NotSupportedException] "The application type could not be determined." }
		}

		if (Get-Service $application.Manifest.Id -ErrorAction Ignore) {
			throw [InvalidOperationException] "The service ""$($application.Manifest.Id)"" already exists."
		}

		$properties = [ordered]@{
			AppDirectory = $application.Path
			AppEnvironmentExtra = "$($application.EnvironmentVariable)=$($application.Manifest.Environment)"
			AppNoConsole = "1"
			AppStderr = Join-Path $application.Path var/Error.log
			AppStdout = Join-Path $application.Path var/Output.log
			Description = $application.Manifest.Description
			DisplayName = $application.Manifest.Name
			Start = "SERVICE_AUTO_START"
		}

		$programPath = (Get-Command $application.Program).Path
		if ($application.Is32Bit -and $IsWindows) { $programPath = $programPath -replace "\\Program Files\\", "\Program Files (x86)\" }

		$nssm = (Get-Command nssm -ErrorAction Ignore) ?? (Get-NssmPath)
		& $nssm install $application.Manifest.Id $programPath $application.EntryPoint | Out-Null
		$properties.Keys | ForEach-Object { & $nssm set $application.Manifest.Id $_ $properties.$_ | Out-Null }

		if ($Credential) {
			$password = $Credential.Password.Length ? (ConvertFrom-SecureString $Credential.Password -AsPlainText) : ""
			& $nssm set $application.Manifest.Id ObjectName $Credential.UserName $password | Out-Null
		}

		if ($Start) { Start-Service $application.Manifest.Id }
		$created = $Start ? "started" : "created"
		"The service ""$($application.Manifest.Id)"" has been successfully $created."
	}
}

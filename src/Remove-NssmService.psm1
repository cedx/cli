using namespace System.Diagnostics.CodeAnalysis
using module ./Nssm/DotNetApplication.psm1
using module ./Nssm/NodeApplication.psm1
using module ./Nssm/PowerShellApplication.psm1
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
	[SuppressMessage("PSUseShouldProcessForStateChangingFunctions", "")]
	param (
		[Parameter(Position = 0, ValueFromPipeline)]
		[ValidateScript({ Test-Path $_ -PathType Container }, ErrorMessage = "The specified directory does not exist.")]
		[string] $Path = $PWD
	)

	begin {
		if (-not $IsWindows) { throw [PlatformNotSupportedException] "This command only supports the Windows platform." }
		if (-not (Test-Privilege)) { throw [UnauthorizedAccessException] "You must run this command in an elevated prompt." }
	}

	process {
		$application = switch ($true) {
			((Test-Path "$Path/src/Server/*.cs") -or (Test-Path "$Path/src/*.cs")) { [DotNetApplication]::new($Path); break }
			((Test-Path "$Path/src/Server/*.psm1") -or (Test-Path "$Path/src/*.psm1")) { [PowerShellApplication]::new($Path); break }
			((Test-Path "$Path/src/Server/*.ts") -or (Test-Path "$Path/src/*.ts")) { [NodeApplication]::new($Path); break }
			default { throw [NotSupportedException] "The application type could not be determined." }
		}

		if (-not (Get-Service $application.Id -ErrorAction Ignore)) {
			throw [InvalidOperationException] "The service ""$($application.Id)"" does not exist."
		}
		else {
			Stop-Service $application.Id
			Remove-Service $application.Id
			Write-Output "The service ""$($application.Id)"" has been successfully removed."
		}
	}
}

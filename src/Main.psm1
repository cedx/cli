<#
.SYNOPSIS
	Returns the version number of the application.
.OUTPUTS
	The version number of the application.
#>
function Get-CliVersion {
	[CmdletBinding()]
	[OutputType([semver])] param ()

	$module = Import-PowerShellDataFile "$PSScriptRoot/../Cli.psd1"
	[semver] $module.ModuleVersion
}

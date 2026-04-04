<#
.SYNOPSIS
	Returns the version number of the application.
.OUTPUTS
	The version number of the application.
#>
function Get-CliVersion {
	[CmdletBinding()]
	[OutputType([string])]
	[OutputType([version])]
	param (
		# Value indicating whether to return a `[version]` object.
		[switch] $PassThru
	)

	$module = "$PSScriptRoot/../Belin.Cli.psd1"
	$version = (Import-PowerShellDataFile ((Test-Path $module) ? $module : "$PSScriptRoot/../Cli.psd1")).ModuleVersion
	$PassThru ? [version] $version : "Cédric Belin's CLI $version"
}

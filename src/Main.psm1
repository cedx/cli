<#
.SYNOPSIS
	Returns the version number of the application.
.OUTPUTS
	The version number of the application.
#>
function Get-CliVersion {
	[OutputType([string])] param ()
	(Import-PowerShellDataFile "$PSScriptRoot/../Cli.psd1").ModuleVersion
}

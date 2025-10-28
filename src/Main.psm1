<#
.SYNOPSIS
	Returns the application version number.
.OUTPUTS
	The application version number.
#>
function Get-CliVersion {
	[OutputType([string])] param ()
	(Import-PowerShellDataFile $PSScriptRoot/../Cli.psd1).ModuleVersion
}

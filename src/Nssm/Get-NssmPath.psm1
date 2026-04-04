<#
.SYNOPSIS
	Returns the path of the `nssm` program according to the specified process architecture.
.INPUTS
	The process architecture.
.OUTPUTS
	The absolute path of the `nssm` program.
#>
function Get-NssmPath {
	[CmdletBinding()]
	[OutputType([string])]
	param (
		# The process architecture.
		[Parameter(Position = 0, ValueFromPipeline)]
		[ValidateSet("x64", "x86")]
		[string] $Architecture = [Environment]::Is64BitOperatingSystem ? "x64" : "x86"
	)

	Join-Path $PSScriptRoot "../../res/Nssm/nssm.$Architecture.exe" -Resolve
}

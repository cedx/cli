using module ../../src/Diagnostics/Get-ExecutableArchitecture.psm1

<#
.SYNOPSIS
	Tests the features of the `Get-ExecutableArchitecture` cmdlet.
#>
Describe "Get-ExecutableArchitecture" {
	It "should return the architecture of the given executable" -ForEach "x64", "x86" {
		"$PSScriptRoot/../../res/Nssm/nssm.$_.exe" | Get-ExecutableArchitecture | Should -Be $_
	}
}

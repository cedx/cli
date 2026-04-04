using module ../../src/Nssm/Get-NssmPath.psm1

<#
.SYNOPSIS
	Tests the features of the `Get-NssmPath` cmdlet.
#>
Describe "Get-NssmPath" {
	It "should return the path of the ``nssm`` program according to the given process architecture" -ForEach "x64", "x86" {
		$pattern = "*/res/Nssm/nssm.{0}.exe" -replace "/", ($IsWindows ? "\" : "/")
		Get-NssmPath -Architecture $_ | Should -BeLikeExactly ($pattern -f $_)
	}
}

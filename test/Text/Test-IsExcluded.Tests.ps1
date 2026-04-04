using module ../../src/Text/Test-IsExcluded.psm1

<#
.SYNOPSIS
	Tests the features of the `Test-IsExcluded` cmdlet.
#>
Describe "Test-IsExcluded" {
	It "should return `$false if the file path does not contain any excluded folder" -ForEach @(
		"C:\Users\Cedric\.gitconfig"
		"/usr/local/bin/pwsh"
	) {
		$_ | Test-IsExcluded | Should -BeFalse
	}

	It "should return `$true if the file path contains an excluded folder" -ForEach @(
		"C:\Projects\Cli\.git\config"
		"/var/www/ps_modules/Pester/Pester.ps1"
	) {
		$_ | Test-IsExcluded | Should -BeTrue
	}
}

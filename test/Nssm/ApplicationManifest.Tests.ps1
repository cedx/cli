using module ../../src/Nssm/ApplicationManifest.psm1

<#
.SYNOPSIS
	Tests the features of the `ApplicationManifest` class.
#>
Describe "ApplicationManifest" {
	Context "Read" {
		It "should support the JSON manifests" -ForEach "json", "psd1", "xml" {
			$manifest = [ApplicationManifest]::Read("$PSScriptRoot/../../res/UnitTesting/Manifest.$_")
			$manifest.Description | Should -BeNullOrEmpty
			$manifest.Environment | Should -BeNullOrEmpty
			$manifest.Id | Should -BeExactly "MyApp"
			$manifest.Name | Should -BeExactly "My Application 1.0"
		}
	}
}

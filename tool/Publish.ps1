. $PSScriptRoot/Default.ps1

Write-Output "Publishing the package..."
$version = (Import-PowerShellDataFile "Cli.psd1").ModuleVersion
git tag "v$version"
git push origin "v$version"

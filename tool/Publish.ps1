. $PSScriptRoot/Default.ps1

Write-Output "Publishing the package..."
$version = (Import-PowerShellDataFile "Package.psd1").Version
git tag "v$version"
git push origin "v$version"

. $PSScriptRoot/Default.ps1

Write-Host "Publishing the package..."
$version = Select-Xml "//Version" Package.xml
git tag "v$version"
git push origin "v$version"

. $PSScriptRoot/Default.ps1

Write-Host "Publishing the package..."
$version = (Select-Xml "//Version" Package.xml).Node.InnerText
git tag "v$version"
git push origin "v$version"

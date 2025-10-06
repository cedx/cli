. tool/Default.ps1

Write-Host "Publishing the package..."
$version = [xml] (Get-Content "Package.xml") | Select-Xml "//Version"
git tag "v$version"
git push origin "v$version"

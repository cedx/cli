Write-Host "Updating the version number in the sources..."
$version = [xml] (Get-Content "Package.xml") | Select-Xml "//Version"
(Get-Content "ReadMe.md") -replace "project/v\d+(\.\d+){2}", "project/v$version" | Out-File "ReadMe.md"
(Get-Content "Setup.iss") -replace 'version "\d+(\.\d+){2}"', "version ""$version""" | Out-File "Setup.iss"
foreach ($item in Get-ChildItem "*/*.csproj") {
	(Get-Content $item) -replace "<Version>\d+(\.\d+){2}</Version>", "<Version>$version</Version>" | Out-File $item
}

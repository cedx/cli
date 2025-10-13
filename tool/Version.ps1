Write-Output "Updating the version number in the sources..."
$version = (Import-PowerShellDataFile "Cli.psd1").ModuleVersion
(Get-Content "ReadMe.md") -replace "project/v\d+(\.\d+){2}", "project/v$version" | Out-File "ReadMe.md"
(Get-Content "Setup.iss") -replace 'version "\d+(\.\d+){2}"', "version ""$version""" | Out-File "Setup.iss"
foreach ($item in Get-ChildItem "*/*.csproj") {
	(Get-Content $item) -replace "<Version>\d+(\.\d+){2}</Version>", "<Version>$version</Version>" | Out-File $item
}

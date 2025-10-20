"Updating the version number in the sources..."
$version = (Import-PowerShellDataFile "Cli.psd1").ModuleVersion
(Get-Content "ReadMe.md") -replace "module/v\d+(\.\d+){2}", "module/v$version" | Out-File "ReadMe.md"
(Get-Content "Setup.iss") -replace 'version "\d+(\.\d+){2}"', "version ""$version""" | Out-File "Setup.iss"

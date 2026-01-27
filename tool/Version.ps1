"Updating the version number in the sources..."
$version = Import-PowerShellDataFile Cli.psd1 | Select-Object -ExpandProperty ModuleVersion
(Get-Content Cli.csproj) -replace "<Version>\d+(\.\d+){2}</Version>", "<Version>$version</Version>" | Out-File Cli.csproj

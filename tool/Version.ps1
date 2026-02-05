"Updating the version number in the sources..."
$version = Import-PowerShellDataFile Cli.psd1 | Select-Object -ExpandProperty ModuleVersion
(Get-Content Cli.csproj -Raw) -replace "<Version>\d+(\.\d+){2}</Version>", "<Version>$version</Version>" | Set-Content Cli.csproj -NoNewline

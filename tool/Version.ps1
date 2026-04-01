"Updating the version number in the sources..."
$version = (Import-PowerShellDataFile Cli.psd1).ModuleVersion
(Get-Content src/Cli.csproj -Raw) -replace "<Version>\d+(\.\d+){2}</Version>", "<Version>$version</Version>" | Set-Content src/Cli.csproj -NoNewline

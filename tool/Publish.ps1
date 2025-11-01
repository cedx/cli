. "$PSScriptRoot/Default.ps1"

"Publishing the package..."
$module = Get-Item "Cli.psd1"
$version = (Import-PowerShellDataFile $module).ModuleVersion
git tag "v$version"
git push origin "v$version"

$output = "var/$($module.BaseName)"
New-Item $output -ItemType Directory | Out-Null
Copy-Item $module $output
Copy-Item *.md $output
Copy-Item bin $output -Recurse
Copy-Item src $output -Recurse

Compress-PSResource $output var
Publish-PSResource -ApiKey $Env:PSGALLERY_API_KEY -NupkgPath "var/$($module.BaseName).$version.nupkg"

if ($Release) { & "$PSScriptRoot/Default.ps1" }
else {
	"The ""-Release"" switch must be set!"
	exit 1
}

"Publishing the module..."
$module = Import-PowerShellDataFile "Cli.psd1"
$version = $module.ModuleVersion
git tag "v$version"
git push origin "v$version"

$name = Split-Path "Cli.psd1" -LeafBase
$output = "var/$name"
New-Item $output/bin -ItemType Directory | Out-Null
Copy-Item $name.psd1 $output
Copy-Item *.md $output
Copy-Item src $output -Filter *.psm1 -Recurse
Copy-Item $module.RootModule $output/bin
if ("RequiredAssemblies" -in $module.Keys) { Copy-Item $module.RequiredAssemblies $output/bin }

Compress-PSResource $output var
Publish-PSResource -ApiKey $Env:PSGALLERY_API_KEY -NupkgPath "var/$name.$version.nupkg"

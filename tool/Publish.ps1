if ($Release) { & "$PSScriptRoot/Default.ps1" }
else {
	"The ""-Release"" switch must be set!"
	exit 1
}

"Publishing the module..."
$module = Import-PowerShellDataFile Cli.psd1
$version = $module.ModuleVersion
git tag "v$version"
git push origin "v$version"

$output = "var/PSModule"
New-Item $output/bin -ItemType Directory | Out-Null
Copy-Item Cli.psd1 $output/Belin.Cli.psd1
Copy-Item *.md $output
Copy-Item res, src $output -Recurse
Remove-Item $output/res/UnitTesting, $output/src/*.cs*, $output/src/obj -Recurse
$module.RequiredAssemblies | Copy-Item -Destination $output/bin

$output = "var/PSGallery"
New-Item $output -ItemType Directory | Out-Null
Compress-PSResource var/PSModule $output
Get-Item "$output/*.nupkg" | ForEach-Object { Publish-PSResource -ApiKey $Env:PSGALLERY_API_KEY -NupkgPath $_ -Repository PSGallery }

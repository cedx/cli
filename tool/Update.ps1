"Updating the dependencies..."
$modules = Import-PowerShellDataFile PSModules.psd1
$modules.PSGallery.Keys | ForEach-Object { Update-PSResource $_ -TrustRepository }

New-Item bin -Force -ItemType Directory | Out-Null
$modules.NuGet.Keys | ForEach-Object {
	$package = Install-Package $_ -Destination var -Force -SkipDependencies -Source https://www.nuget.org/api/v2
	Copy-Item "var/$($package.Name).$($package.Version)/lib/net9.0/$($package.Name).dll" "bin/$($package.Name).dll"
}

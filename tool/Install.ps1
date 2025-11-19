"Installing the dependencies..."
$modules = Import-PowerShellDataFile PSModules.psd1
Install-PSResource -RequiredResource $modules.PSGallery -TrustRepository -WarningAction Ignore

New-Item lib -Force -ItemType Directory | Out-Null
$modules.NuGet.Keys | ForEach-Object {
	$package = Install-Package $_ -Destination var -Force -SkipDependencies -Source https://www.nuget.org/api/v2
	Copy-Item "var/$($package.Name).$($package.Version)/lib/net9.0/$($package.Name).dll" "lib/$($package.Name).dll"
}

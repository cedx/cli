. $PSScriptRoot/Assets.ps1

"Building the project..."
$configuration = $release ? "Release" : "Debug"
dotnet build Cli.slnx --configuration=$configuration

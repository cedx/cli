. $PSScriptRoot/Assets.ps1

Write-Host "Building the project..."
$configuration = $release ? "Release" : "Debug"
dotnet build Cli.slnx --configuration=$configuration

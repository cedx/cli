. $PSScriptRoot/Assets.ps1

"Building the solution..."
dotnet build --configuration ($Release ? "Release" : "Debug")

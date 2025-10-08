. $PSScriptRoot/Assets.ps1

Write-Host "Watching for file changes..."
Start-Process dotnet "watch build" -NoNewWindow -Wait -WorkingDirectory src

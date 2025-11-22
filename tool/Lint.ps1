"Performing the static analysis of source code..."
Import-Module PSScriptAnalyzer
Invoke-ScriptAnalyzer $PSScriptRoot -Recurse
Invoke-ScriptAnalyzer src -Recurse
Test-ModuleManifest "$PSScriptRoot/../Cli.psd1" | Out-Null

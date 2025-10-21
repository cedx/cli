#!/usr/bin/env pwsh
param (
	[Parameter(Mandatory, Position = 0)]
	[ValidateSet("Iconv", "MySql", "Nssm", "Setup")]
	[string] $Command
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$commandPath = Get-Item $PSCommandPath
$scriptRoot = $commandPath.LinkType ? (Split-Path $commandPath.LinkTarget) : $PSScriptRoot

$script = "$scriptRoot/src/$Command.ps1"
if (Test-Path $script) { . $script @args }
else { Write-Error "TODO" }

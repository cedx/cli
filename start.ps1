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
. $scriptRoot/src/$Command.ps1 @rgs

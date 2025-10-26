#!/usr/bin/env pwsh
param (
	[Parameter(Mandatory, Position = 0)] [string] $Command,
	[Parameter(Position = 1, ValueFromRemainingArguments)] [string[]] $Arguments
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$commandPath = Get-Item $PSCommandPath
$scriptRoot = $commandPath.LinkType ? (Split-Path $commandPath.LinkTarget) : $PSScriptRoot
Import-Module $scriptRoot/Cli.psd1
Invoke-Expression "$Command $Arguments"

#!/usr/bin/env pwsh
param (
	[Parameter(Mandatory, Position = 0)]
	[ArgumentCompleter({
		param ($commandName, $parameterName, $wordToComplete)
		(Get-Item "$PSScriptRoot/src/$wordToComplete*.psm1").BaseName
	})]
	[string] $Command,

	[Parameter(Position = 1, ValueFromRemainingArguments)]
	[string[]] $Arguments
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version Latest

$commandPath = Get-Item $PSCommandPath
$scriptRoot = $commandPath.LinkType ? (Split-Path $commandPath.LinkTarget) : $PSScriptRoot
Import-Module $scriptRoot/Cli.psd1
Invoke-Expression "$Command $Arguments"

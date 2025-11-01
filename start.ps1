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

$scriptBlock = {
	param (
		[Parameter(Mandatory, Position = 0)] [string] $Command,
		[Parameter(Position = 1, ValueFromRemainingArguments)] [string[]] $Arguments
	)

	$ErrorActionPreference = "Stop"
	$PSNativeCommandUseErrorActionPreference = $true
	Set-StrictMode -Version Latest
	Import-Module ./Cli.psd1
	& $Command @Arguments
}

$scriptArgs = (@($Command) + $Arguments)
pwsh -Command $scriptBlock -args $scriptArgs

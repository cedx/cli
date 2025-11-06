#!/usr/bin/env pwsh
param (
	[Parameter(Mandatory, Position = 0)]
	[ArgumentCompleter({
		param ($commandName, $parameterName, $wordToComplete)
		(Get-Item "$PSScriptRoot/src/$wordToComplete*.psm1" -Exclude "Main.psm1").BaseName
	})]
	[string] $Command,

	[Parameter(Position = 1, ValueFromRemainingArguments)]
	[string[]] $Arguments
)

pwsh -Command @"
`$ErrorActionPreference = "Stop"
`$PSNativeCommandUseErrorActionPreference = `$true
Set-StrictMode -Version Latest
Import-Module "$PSScriptRoot/Cli.psd1"
$Command $($Arguments.ForEach{ $_.Contains(" ") ? """$_""" : $_ })
"@

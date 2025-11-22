#!/usr/bin/env pwsh
param (
	[Parameter(Mandatory, Position = 0)]
	[ArgumentCompleter({
		param ($commandName, $parameterName, $wordToComplete)
		(Get-Item "$PSScriptRoot/src/$wordToComplete*.psm1" -Exclude "Main.psm1").BaseName
	})]
	[string] $Command,

	[Parameter(Position = 1, ValueFromRemainingArguments)]
	[string[]] $Parameters
)

$scriptBlock = {
	$ErrorActionPreference = "Stop"
	$PSNativeCommandUseErrorActionPreference = $true
	Set-StrictMode -Version Latest

	$scriptRoot, $command, $parameters = $args
	Import-Module "$scriptRoot/Cli.psd1"
	Invoke-Expression "$command $($parameters.ForEach{ $_.Contains(" ") ? "'$_'" : $_ })"
}

pwsh -Command $scriptBlock -args $PSScriptRoot, $Command, $Parameters

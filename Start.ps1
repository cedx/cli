#!/usr/bin/env pwsh
param (
	# The name of the cmdlet to run.
	[Parameter(Mandatory, Position = 0)]
	[ArgumentCompleter({
		param ($commandName, $parameterName, $wordToComplete)
		$module = Import-PowerShellDataFile "$PSScriptRoot/Cli.psd1"
		($module.CmdletsToExport + $module.FunctionsToExport) -like "$wordToComplete*"
	})]
	[string] $Command,

	# The parameters of the cmdlet to run.
	[Parameter(Position = 1, ValueFromRemainingArguments)]
	[string[]] $Parameters = @()
)

$scriptBlock = {
	$ErrorActionPreference = "Stop"
	$PSNativeCommandUseErrorActionPreference = $true
	Set-StrictMode -Version Latest

	$scriptRoot, $command, $parameters = $args
	Import-Module "$scriptRoot/Cli.psd1"

	$argumentList = $parameters | ForEach-Object { $_.Contains(" ") ? "'$_'" : $_ }
	Invoke-Expression "$command $($argumentList -join " ")"
}

pwsh -Command $scriptBlock -args $PSScriptRoot, $Command, $Parameters

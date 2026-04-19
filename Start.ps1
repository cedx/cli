#!/usr/bin/env pwsh
param (
	# The name of the cmdlet to run.
	[Parameter(Mandatory, Position = 0)]
	[ArgumentCompleter({
		param ([string] $commandName, [string] $parameterName, [string] $wordToComplete)
		$module = Import-PowerShellDataFile "$PSScriptRoot/Cli.psd1"
		($module.CmdletsToExport + $module.FunctionsToExport) -like "$wordToComplete*"
	})]
	[ValidateScript({
		$module = Import-PowerShellDataFile "$PSScriptRoot/Cli.psd1"
		($module.CmdletsToExport + $module.FunctionsToExport) -contains $_
	}, ErrorMessage = "The specified command does not exist.")]
	[string] $Command,

	# The parameters of the cmdlet to run.
	[Parameter(Position = 1, ValueFromRemainingArguments)]
	[string[]] $Parameters = @()
)

$scriptBlock = {
	$ErrorActionPreference = "Stop"
	$PSNativeCommandUseErrorActionPreference = $true

	$scriptRoot, $command, $parameters = $args
	Import-Module "$scriptRoot/Cli.psd1"

	$argumentList = $parameters.ForEach{ $_.Contains(" ") ? "'$_'" : $_ }
	Invoke-Expression "$command $($argumentList -join " ")"
}

pwsh -Command $scriptBlock -args $PSScriptRoot, $Command, $Parameters

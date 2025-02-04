#!/usr/bin/env pwsh
Set-StrictMode -Version Latest
$linkType = (Get-Item $PSCommandPath).LinkType
$scriptRoot = $linkType ? (Split-Path (Get-ChildItem $PSCommandPath).LinkTarget) : $PSScriptRoot
& "$scriptRoot/bin/Belin.Cli.exe" @args

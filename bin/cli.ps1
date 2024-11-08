#!/usr/bin/env pwsh
Set-StrictMode -Version Latest

$file = "$PSScriptRoot/cli.js"
if (Test-Path "$PSScriptRoot/../lib/cli.js.map") { node --enable-source-maps $file @args }
else { node $file @args }

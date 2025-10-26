using assembly ../bin/MySqlConnector.dll

<#
.SYNOPSIS
	Alters the character set of MariaDB/MySQL tables.
#>
function Set-MySqlCharset {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Position = 0)]
		[ValidateScript({ Test-Path $_ -IsValid })]
		[string] $Path = "C:\Program Files\PHP",

		[Parameter()]
		[switch] $RegisterEventSource
	)

	end {
	}
}

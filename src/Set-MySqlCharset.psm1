using assembly ../bin/MySqlConnector.dll

<#
.SYNOPSIS
	Alters the character set of MariaDB/MySQL tables.
.PARAMETER Uri
	The connection URI.
#>
function Set-MySqlCharset {
	[CmdletBinding()]
	[OutputType([void])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[ValidateScript({ $_.IsAbsoluteUri -and ($_.Scheme -in "mariadb", "mysql") -and $_.UserInfo.Contains(":") })]
		[uri] $Uri,

		[Parameter()]
		[string] $Schema = "",

		[Parameter()]
		[string[]] $Table = @()
	)

}

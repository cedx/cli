using assembly ../bin/MySqlConnector.dll

<#
.SYNOPSIS
	Alters the storage engine of MariaDB/MySQL tables.
.PARAMETER Uri
	The connection URI.
#>
function Set-MySqlEngine {
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

using namespace MySqlConnector
using namespace System.Diagnostics.CodeAnalysis

<#
.SYNOPSIS
	Creates a new database connection.
.PARAMETER Uri
	The connection URI.
.PARAMETER Open
	Value indicating whether to open the connection.
.OUTPUTS
	The newly created connection.
#>
function New-Connection {
	[OutputType([MySqlConnection])]
	[SuppressMessage("PSUseShouldProcessForStateChangingFunctions", "")]
	param (
		[Parameter(Mandatory, Position = 0)]
		[ValidateScript(
			{ $_.IsAbsoluteUri -and ($_.Scheme -in "mariadb", "mysql") -and $_.UserInfo.Contains(":") },
			ErrorMessage = "The connection URI is invalid."
		)]
		[uri] $Uri,

		[Parameter()]
		[switch] $Open
	)

	$userInfo = ($Uri.UserInfo -split ":").ForEach{ [Uri]::UnescapeDataString($_) }
	$builder = [MySqlConnectionStringBuilder]@{
		Server = $Uri.Host
		Port = $Uri.IsDefaultPort ? 3306 : $Uri.Port
		Database = "information_schema"
		UserID = $userInfo[0]
		Password = $userInfo[1]
		ConvertZeroDateTime = $true
		Pooling = $false
		UseCompression = $Uri.Host -notin "::1", "127.0.0.1", "localhost"
	}

	$connection = [MySqlConnection] $builder.ConnectionString
	if ($Open) { $connection.Open() }
	$connection
}

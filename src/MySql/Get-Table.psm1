using namespace MySqlConnector
using namespace System.Collections.Generic
using module ./Schema.psm1
using module ./Table.psm1

<#
.SYNOPSIS
	Gets the list of tables contained in the specified schema.
.PARAMETER Connection
	The database connection.
.PARAMETER Schema
	The database schema.
.OUTPUTS
	The tables contained in the specified schema.
#>
function Get-Table {
	[OutputType([Table[]])]
	param (
		[Parameter(Mandatory, Position = 0)] [ValidateNotNull()] [MySqlConnection] $Connection,
		[Parameter(Mandatory, Position = 1)] [ValidateNotNull()] [Schema] $Schema
	)

	$sql = "
		SELECT *
		FROM information_schema.TABLES
		WHERE TABLE_SCHEMA = @Name AND TABLE_TYPE = @Type
		ORDER BY TABLE_NAME"

	$command = [MySqlCommand]::new($sql, $Connection)
	$command.Parameters.AddWithValue("@Name", $Schema.Name) | Out-Null
	$command.Parameters.AddWithValue("@Type", [TableType]::BaseTable) | Out-Null
	$reader = $command.ExecuteReader()

	$list = [List[Table]]::new()
	while ($reader.Read()) { $list.Add([Table]::new($reader)) }
	$reader.Close()
	$list.ToArray()
}

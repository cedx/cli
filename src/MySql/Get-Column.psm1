using namespace MySqlConnector
using namespace System.Collections.Generic
using module ./Column.psm1
using module ./Table.psm1

<#
.SYNOPSIS
	Gets the list of columns contained in the specified table.
.PARAMETER Connection
	The database connection.
.PARAMETER Table
	The database table.
.OUTPUTS
	The columns contained in the specified table.
#>
function Get-Column {
	[OutputType([Column[]])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[MySqlConnection] $Connection,

		[Parameter(Mandatory, Position = 1)]
		[Table] $Table
	)

	$sql = "
		SELECT *
		FROM information_schema.COLUMNS
		WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @Name
		ORDER BY ORDINAL_POSITION"

	$command = [MySqlCommand]::new($sql, $Connection)
	$command.Parameters.AddWithValue("@Name", $Table.Name) | Out-Null
	$command.Parameters.AddWithValue("@Schema", $Table.Schema) | Out-Null
	$reader = $command.ExecuteReader()

	$list = [List[Column]]::new()
	while ($reader.Read()) { $list.Add([Column]::new($reader)) }
	$reader.Close()
	$list.ToArray()
}

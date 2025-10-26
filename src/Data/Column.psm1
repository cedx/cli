using namespace System.ComponentModel.DataAnnotations.Schema

<#
.SYNOPSIS
	Provides the metadata of a table column.
#>
[Table("COLUMNS")]
class Column {

	<#
	.SYNOPSIS
		The column name.
	#>
	[Column("COLUMN_NAME")]
	[ValidateNotNullOrWhiteSpace()]
	[string] $Name

	<#
	.SYNOPSIS
		The column position.
	#>
	[Column("ORDINAL_POSITION")]
	[int] $Position = 0

	<#
	.SYNOPSIS
		The schema containing this column.
	#>
	[Column("TABLE_SCHEMA")]
	[string] $Schema = ""

	<#
	.SYNOPSIS
		The table containing this column.
	#>
	[Column("TABLE_NAME")]
	[string] $Table = ""

	<#
	.SYNOPSIS
		Creates a new column.
	.PARAMETER Name
		The column name.
	#>
	Column([string] $Name) {
		$this.Name = $Name
	}
}

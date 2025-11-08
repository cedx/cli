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
	[ColumnAttribute("COLUMN_NAME")]
	[string] $Name = ""

	<#
	.SYNOPSIS
		The column position.
	#>
	[ColumnAttribute("ORDINAL_POSITION")]
	[int] $Position = 0

	<#
	.SYNOPSIS
		The schema containing this column.
	#>
	[ColumnAttribute("TABLE_SCHEMA")]
	[string] $Schema = ""

	<#
	.SYNOPSIS
		The table containing this column.
	#>
	[ColumnAttribute("TABLE_NAME")]
	[string] $Table = ""

	<#
	.SYNOPSIS
		Creates a new column.
	#>
	Column() {}

	<#
	.SYNOPSIS
		Creates a new column from the specified data record.
	.PARAMETER DataRecord
		The data record providing the object values.
	#>
	Column([hashtable] $DataRecord) {
		$this.Name = $DataRecord.COLUMN_NAME
		$this.Position = $DataRecord.ORDINAL_POSITION
		$this.Schema = $DataRecord.TABLE_SCHEMA
		$this.Table = $DataRecord.TABLE_NAME
	}
}

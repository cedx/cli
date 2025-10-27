using namespace System.ComponentModel.DataAnnotations.Schema
using namespace System.Data

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
	[string] $Name = ""

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
		Creates a new table from the specified data record.
	.PARAMETER DataRecord
		A data record providing values to initialize the instance.
	.OUTPUTS
		The newly created table.
	#>
	static [Column] OfRecord([IDataRecord] $DataRecord) {
		return [Column]@{
			Name = $DataRecord["COLUMN_NAME"]
			Position = $DataRecord["ORDINAL_POSITION"]
			Schema = $DataRecord["TABLE_SCHEMA"]
			Table = $DataRecord["TABLE_NAME"]
		}
	}
}

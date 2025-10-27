using namespace System.ComponentModel.DataAnnotations.Schema
using namespace System.Data

<#
.SYNOPSIS
	Provides the metadata of a database table.
#>
[TableAttribute("TABLES")]
class Table {

	<#
	.SYNOPSIS
		The default collation.
	#>
	[Column("TABLE_COLLATION")]
	[string] $Collation = ""

	<#
	.SYNOPSIS
		The storage engine.
	#>
	[Column("ENGINE")]
	[string] $Engine = [TableEngine]::None

	<#
	.SYNOPSIS
		The table name.
	#>
	[Column("TABLE_NAME")]
	[string] $Name = ""

	<#
	.SYNOPSIS
		The schema containing this table.
	#>
	[Column("TABLE_SCHEMA")]
	[string] $Schema = ""

	<#
	.SYNOPSIS
		The table type.
	#>
	[Column("TABLE_TYPE")]
	[string] $Type = [TableType]::BaseTable

	<#
	.SYNOPSIS
		Gets the fully qualified name.
	.PARAMETER Escape
		Value indicating whether to escape the SQL identifiers.
	.OUTPUTS
		The fully qualified name.
	#>
	[string] QualifiedName([bool] $Escape = $false) {
		$scriptBlock = $Escape ? { "``$_``" } : { $_ }
		return "$($scriptBlock.Invoke($this.Schema)).$($scriptBlock.Invoke($this.Name))"

		# TODO use https://mysqlconnector.net/api/mysqlconnector/mysqlcommandbuilder/quoteidentifier/
	}

	<#
	.SYNOPSIS
		Creates a new table from the specified data record.
	.PARAMETER DataRecord
		A data record providing values to initialize the instance.
	.OUTPUTS
		The newly created table.
	#>
	static [Table] OfRecord([IDataRecord] $DataRecord) {
		return [Table]@{
			Collation = $DataRecord["TABLE_COLLATION"]
			Engine = $DataRecord["ENGINE"]
			Name = $DataRecord["TABLE_NAME"]
			Schema = $DataRecord["TABLE_SCHEMA"]
			Type = $DataRecord["TABLE_TYPE"]
		}
	}
}

<#
.SYNOPSIS
	Defines the storage engine of a table.
#>
class TableEngine {

	<#
	.SYNOPSIS
		The table does not use any storage engine.
	#>
	static [string] $None = ""

	<#
	.SYNOPSIS
		The storage engine is Aria.
	#>
	static [string] $Aria = "Aria"

	<#
	.SYNOPSIS
		The storage engine is InnoDB.
	#>
	static [string] $InnoDB = "InnoDB"

	<#
	.SYNOPSIS
		The storage engine is MyISAM.
	#>
	static [string] $MyISAM = "MyISAM"
}

<#
.SYNOPSIS
	Defines the type of a table.
#>
class TableType {

	<#
	.SYNOPSIS
		A base table.
	#>
	static [string] $BaseTable = "BASE TABLE"

	<#
	.SYNOPSIS
		A view.
	#>
	static [string] $View = "VIEW"
}

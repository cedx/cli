using namespace System.ComponentModel.DataAnnotations.Schema

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
		Creates a new table.
	#>
	Table() {}

	<#
	.SYNOPSIS
		Creates a new table from the specified data record.
	.PARAMETER DataRecord
		The data record providing the object values.
	#>
	Table([hashtable] $DataRecord) {
		$this.Collation = $DataRecord.TABLE_COLLATION
		$this.Engine = $DataRecord.ENGINE
		$this.Name = $DataRecord.TABLE_NAME
		$this.Schema = $DataRecord.TABLE_SCHEMA
		$this.Type = $DataRecord.TABLE_TYPE
	}

	<#
	.SYNOPSIS
		Gets the fully qualified name.
	.OUTPUTS
		The fully qualified name.
	#>
	[string] QualifiedName() {
		return $this.QualifiedName($false)
	}

	<#
	.SYNOPSIS
		Gets the fully qualified name.
	.PARAMETER Escape
		Value indicating whether to escape the SQL identifiers.
	.OUTPUTS
		The fully qualified name.
	#>
	[string] QualifiedName([bool] $Escape) {
		$scriptBlock = $Escape ? { "``$($args[0])``" } : { $args[0] }
		return "$(& $scriptBlock $this.Schema).$(& $scriptBlock $this.Name)"
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

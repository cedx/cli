using namespace System.ComponentModel.DataAnnotations.Schema

<#
.SYNOPSIS
	Provides the metadata of a database table.
#>
[Table("TABLES")]
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
	[ValidateNotNullOrWhiteSpace()]
	[string] $Name

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
	.PARAMETER Name
		The table name.
	#>
	Table([string] $Name) {
		$this.Name = $Name
	}

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

<#
.SYNOPSIS
	Defines the format of the output files.
#>
class BackupFormat {

	<#
	.SYNOPSIS
		The JSON Lines format.
	#>
	static [string] $JsonLines = "jsonl"

	<#
	.SYNOPSIS
		The SQL format.
	#>
	static [string] $SqlDump = "sql"
}

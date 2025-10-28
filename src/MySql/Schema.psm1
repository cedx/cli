using namespace System.ComponentModel.DataAnnotations.Schema
using namespace System.Data

<#
.SYNOPSIS
	Provides the metadata of a database schema.
#>
[Table("SCHEMATA")]
class Schema {

	<#
	.SYNOPSIS
		The default character set.
	#>
	[Column("DEFAULT_CHARACTER_SET_NAME")]
	[string] $Charset = ""

	<#
	.SYNOPSIS
		The default collation.
	#>
	[Column("DEFAULT_COLLATION_NAME")]
	[string] $Collation = ""

	<#
	.SYNOPSIS
		The schema name.
	#>
	[Column("SCHEMA_NAME")]
	[string] $Name = ""

	<#
	.SYNOPSIS
		Creates a new schema from the specified data record.
	.PARAMETER DataRecord
		A data record providing values to initialize the instance.
	.OUTPUTS
		The newly created schema.
	#>
	static [Schema] OfRecord([IDataRecord] $DataRecord) {
		return [Schema]@{
			Charset = $DataRecord["DEFAULT_CHARACTER_SET_NAME"]
			Collation = $DataRecord["DEFAULT_COLLATION_NAME"]
			Name = $DataRecord["SCHEMA_NAME"]
		}
	}
}

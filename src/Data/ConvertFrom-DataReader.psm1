using namespace System.Collections.Generic
using namespace System.Data
using module ./ConvertFrom-DataRecord.psm1

<#
.SYNOPSIS
	Converts the specified data reader to an array of custom objects or hash tables.
.PARAMETER InputObject
	The data reader to convert.
.PARAMETER AsHashtable
	Value indicating whether to convert the data reader to an array of hash tables.
.INPUTS
	The data reader to convert.
.OUTPUTS
	[psobject[]] The array of custom objects corresponding to the specified data reader.
.OUTPUTS
	[ordered[]] The array of hash tables corresponding to the specified data reader.
#>
function ConvertFrom-DataReader {
	[OutputType([object[]])]
	param (
		[Parameter(Mandatory, Position = 0, ValueFromPipeline)]
		[IDataReader] $InputObject,

		[Parameter()]
		[switch] $AsHashtable
	)

	process {
		$list = [List[object]]::new()
		while ($InputObject.Read()) { $list.Add((ConvertFrom-DataRecord $InputObject -AsHashtable:$AsHashtable)) }
		$InputObject.Close()
		$list.ToArray()
	}
}

using namespace System.Data
using module ./ConvertFrom-DataReader.psm1

<#
.SYNOPSIS
	Executes the specified SQL query on a data source.
.PARAMETER Connection
	The connection to the data source.
.PARAMETER Query
	The statement to be executed on the data source.
.PARAMETER Parameters
	The parameters of the SQL query.
.PARAMETER AsHashtable
	Value indicating whether to convert the data reader to an array of hash tables.
.OUTPUTS
	[psobject[]] The array of custom objects returned by the query.
.OUTPUTS
	[ordered[]] The array of hash tables returned by the query.
#>
function Invoke-SqlQuery {
	[OutputType([object[]])]
	param (
		[Parameter(Mandatory, Position = 0)]
		[IDbConnection] $Connection,

		[Parameter(Mandatory, Position = 1)]
		[string] $Query,

		[Parameter(Position = 2)]
		[ValidateNotNull()]
		[hashtable] $Parameters = @{},

		[Parameter()]
		[switch] $AsHashtable
	)

	if ($Connection.State -eq [ConnectionState]::Closed) { $Connection.Open() }

	$command = $null
	try {
		$command = $Connection.CreateCommand()
		$command.CommandText = $Query

		foreach ($key in $Parameters.Keys) {
			$parameter = $command.CreateParameter()
			$parameter.ParameterName = "@$key"
			$parameter.Value = $Parameters.$key
			$command.Parameters.Add($parameter) | Out-Null
		}

		ConvertFrom-DataReader $command.ExecuteReader() -AsHashtable:$AsHashtable
	}
	finally {
		${command}?.Dispose()
	}
}

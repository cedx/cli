using module ./Result.psm1

<#
TODO
#>
function Invoke-NonQuery {
	[OutputType([Result])]
	param ([Parameter(Mandatory, Position = 0)] [ValidateNotNull()] [MySqlCommand] $Command)

	$result = [Result]::Success()
	$reader = $Command.ExecuteReader()
	while ($reader.Read()) {
		switch ($reader["Msg_type"]) {
			"error" { $result.Message = $reader["Msg_text"]; break }
			"note" { $result.Message = $reader["Msg_text"]; break }
			"status" { $result.IsFailure = -not ($result.IsSuccess = $reader["Msg_text"] -eq "OK"); break }
		}
	}

	$reader.Close()
	$result
}

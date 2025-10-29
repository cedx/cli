using module ./Result.psm1

<#
.SYNOPSIS
	Executes an SQL command.
.PARAMETER Command
	The SQL command to execute.
.OUTPUTS
	A value indicating whether the command succeeded or failed.
#>
function Invoke-NonQuery {
	[OutputType([Result])]
	param ([Parameter(Mandatory, Position = 0)] [ValidateNotNull()] [MySqlCommand] $Command)

	$reader = $null
	try {
		$result = [Result]::Success()
		$reader = $Command.ExecuteReader()
		while ($reader.Read()) {
			switch ($reader["Msg_type"]) {
				"error" { $result.Message = $reader["Msg_text"]; break }
				"note" { $result.Message = $reader["Msg_text"]; break }
				"status" { $result.IsFailure = -not ($result.IsSuccess = $reader["Msg_text"] -eq "OK"); break }
			}
		}

		$result
	}
	catch {
		[Result]::Failure($_.ErrorDetails?.Message ?? $_.Exception.Message)
	}
	finally {
		${reader}?.Close()
	}
}

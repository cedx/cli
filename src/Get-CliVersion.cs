namespace Belin.Cli;

/// <summary>
/// Returns the version number of the application.
/// </summary>
[Cmdlet(VerbsCommon.Get, "CliVersion")]
[OutputType(typeof(Version))]
public class GetCliVersion: Cmdlet {

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() =>
		WriteObject(GetType().Assembly.GetName().Version);
}

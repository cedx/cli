namespace Belin.Cli.Nssm;

/// <summary>
/// Returns the path of the <c>nssm</c> program according to the specified process architecture.
/// </summary>
[Cmdlet(VerbsCommon.Get, "NssmPath"), OutputType(typeof(string))]
public class GetNssmPathCommand: Cmdlet {

	/// <summary>
	/// The process architecture.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true), ValidateSet("x64", "x86")]
	public required string Architecture { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() =>
		WriteObject(Path.Join(Path.GetDirectoryName(GetType().Assembly.Location), $"../res/New-NssmService/nssm.{Architecture}.exe"));
}

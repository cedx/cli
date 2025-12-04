namespace Belin.Cli;

using System.Reflection;

/// <summary>
/// Returns the version number of the application.
/// </summary>
[Cmdlet(VerbsCommon.Get, "CliVersion")]
[OutputType(typeof(string)), OutputType(typeof(Version))]
public class GetCliVersionCommand: Cmdlet {

	/// <summary>
	/// Value indicating whether to return a <see cref="Version"/> object.
	/// </summary>
	[Parameter]
	public SwitchParameter PassThru { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() {
		var assembly = GetType().Assembly;
		var product = assembly.GetCustomAttribute<AssemblyProductAttribute>()!.Product;
		var version = assembly.GetName().Version!;
		WriteObject(PassThru ? version : $"{product} {version.ToString(3)}");
	}
}

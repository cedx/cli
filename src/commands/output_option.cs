namespace Belin.Cli.Commands;

/// <summary>
/// Provides the path to an output directory.
/// </summary>
public class OutputOption: Option<string> {

	/// <summary>
	/// Creates a new option.
	/// </summary>
	/// <param name="defaultValue">The default value for the option when it is not specified on the command line.</param>
	public OutputOption(string defaultValue): base(["-o", "--out"], () => defaultValue, "The path to the output directory.") {
		ArgumentHelpName = "directory";
	}
}

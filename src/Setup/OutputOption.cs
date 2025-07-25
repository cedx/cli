namespace Belin.Cli.Setup;

/// <summary>
/// Provides the path to an output directory.
/// </summary>
public class OutputOption: Option<DirectoryInfo> {

	/// <summary>
	/// Creates a new option.
	/// </summary>
	/// <param name="defaultValue">The default value for the option when it is not specified on the command line.</param>
	public OutputOption(DirectoryInfo defaultValue): base("--out", ["-o"]) {
		DefaultValueFactory = _ => defaultValue;
		Description = "The path to the output directory.";
		HelpName = "directory";
	}
}

namespace Belin.Cli.Commands;

/// <summary>
/// Provides the path to an output directory.
/// </summary>
/// <param name="defaultValue">The default value for the option when it is not specified on the command line.</param>
public class OutputOption(string defaultValue): Option<string>(["-o", "--out"], () => defaultValue, "The path to the output directory.") {}

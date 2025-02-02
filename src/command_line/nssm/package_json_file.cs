namespace Belin.Cli.CommandLine.Nssm;

/// <summary>
/// Represents the contents of a <c>package.json</c> file.
/// </summary>
public class PackageJsonFile {

	/// <summary>
	/// The map of package commands.
	/// </summary>
	public Dictionary<string, string>? Bin { get; set; } = null;
}

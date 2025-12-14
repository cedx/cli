namespace Belin.Cli.Nssm;

using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation.Language;

/// <summary>
/// Represents a PowerShell data file.
/// </summary>
/// <param name="hashtable">The values of the PowerShell data file.</param>
public sealed class PowerShellDataFile(Hashtable hashtable):
	ReadOnlyDictionary<string, object?>(hashtable.Cast<DictionaryEntry>().ToDictionary(entry => entry.Key.ToString() ?? "", entry => entry.Value)) {

	/// <summary>
	/// The description of the PowerShell module, if applicable.
	/// </summary>
	public string? Description => TryGetValue("Description", out var value) ? value as string : null;
		
	/// <summary>
	/// The version of the PowerShell module, if applicable.
	/// </summary>
	public Version? ModuleVersion => TryGetValue("ModuleVersion", out var value) && value is string version ? Version.Parse(version) : null;

	/// <summary>
	/// The primary or root file of the PowerShell module, if applicable.
	/// </summary>
	public string? RootModule => TryGetValue("RootModule", out var value) ? value as string : null;

	/// <summary>
	/// Reads the PowerShell data file located at the specified path.
	/// </summary>
	/// <param name="path">The file to open for reading.</param>
	/// <param name="skipLimitCheck">Value indicating whether to skip the hashtable limit validation.</param>
	/// <returns>The data from the file.</returns>
	/// <exception cref="FormatException">The PowerShell data file could not be parsed.</exception>
	public static PowerShellDataFile Read(string path, bool skipLimitCheck = false) {
		var scriptBlockAst = Parser.ParseFile(path, out var tokens, out var errors);
		if (errors.Length > 0) throw new FormatException(errors[0].Message);

		var data = scriptBlockAst.Find(ast => ast is HashtableAst, searchNestedScriptBlocks: false);
		return data is not null
			? new PowerShellDataFile((Hashtable) data.SafeGetValue(skipHashtableSizeCheck: skipLimitCheck))
			: throw new FormatException("The file could not be processed because it is not a valid PowerShell data file.");
	}
}

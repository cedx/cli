namespace Belin.Cli.Nssm;

using System.Collections;
using System.Management.Automation.Language;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Provides information about a web application.
/// </summary>
[XmlRoot("Configuration")]
public sealed class ApplicationManifest {

	/// <summary>
	/// The application description.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// The application environment.
	/// </summary>
	public string? Environment { get; set; }

	/// <summary>
	/// The application identifier.
	/// </summary>
	public string? Id { get; set; }

	/// <summary>
	/// The application name.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Reads the application manifest located at the specified path.
	/// </summary>
	/// <param name="path">The path to the manifest file.</param>
	/// <returns>The application manifest corresponding to the specified file.</returns>
	public static ApplicationManifest? Read(string path) => Path.GetExtension(path).ToLower() switch {
		".json" => ReadJsonManifest(path),
		".psd1" => ReadPowerShellManifest(path),
		".xml" or ".config" => ReadXmlManifest(path),
		_ => null
	};

	/// <summary>
	/// Reads the JSON application manifest located at the specified path.
	/// </summary>
	/// <param name="path">The path to the JSON manifest.</param>
	/// <returns>The application manifest corresponding to the specified JSON file.</returns>
	public static ApplicationManifest? ReadJsonManifest(string path) =>
		JsonSerializer.Deserialize<ApplicationManifest>(File.ReadAllText(path), JsonSerializerOptions.Web);

	/// <summary>
	/// Reads the PowerShell application manifest located at the specified path.
	/// </summary>
	/// <param name="path">The path to the PowerShell manifest.</param>
	/// <returns>The application manifest corresponding to the specified PowerShell file.</returns>
	/// <exception cref="FormatException">The PowerShell manifest could not be parsed.</exception>
	public static ApplicationManifest? ReadPowerShellManifest(string path) {
		var scriptBlockAst = Parser.ParseFile(path, out var tokens, out var errors);
		if (errors.Length > 0) throw new FormatException(errors[0].Message);

		var hashtable = scriptBlockAst.Find(ast => ast is HashtableAst, searchNestedScriptBlocks: false) is Ast ast
			? (Hashtable) ast.SafeGetValue()
			: throw new FormatException("The manifest could not be processed because it is not a valid PowerShell data file.");

		var manifest = new Dictionary<string, object?>(hashtable.Cast<DictionaryEntry>().ToDictionary(entry => entry.Key.ToString() ?? "", entry => entry.Value));
		return new() {
			Description = manifest.TryGetValue("Description", out var description) ? description as string : null,
			Environment = manifest.TryGetValue("Environment", out var environment) ? environment as string : null,
			Id = manifest.TryGetValue("Id", out var id) ? id as string : null,
			Name = manifest.TryGetValue("Name", out var name) ? name as string : null
		};
	}

	/// <summary>
	/// Reads the XML application manifest located at the specified path.
	/// </summary>
	/// <param name="path">The path to the XML manifest.</param>
	/// <returns>The application manifest corresponding to the specified XML file.</returns>
	public static ApplicationManifest? ReadXmlManifest(string path) {
		using var xmlReader = XmlReader.Create(path);
		return (ApplicationManifest?) new XmlSerializer(typeof(ApplicationManifest)).Deserialize(xmlReader);
	}
}

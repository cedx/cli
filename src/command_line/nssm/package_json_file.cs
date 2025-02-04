namespace Belin.Cli.CommandLine.Nssm;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the contents of a <c>package.json</c> file.
/// </summary>
public class PackageJsonFile {

	/// <summary>
	/// The map of package commands.
	/// </summary>
	[JsonPropertyName("bin")]
	public IDictionary<string, string>? Bin { get; set; } = null;

	/// <summary>
	/// The package description.
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; } = null;

	/// <summary>
	/// The package name.
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The version number.
	/// </summary>
	[JsonPropertyName("version")]
	public required string Version { get; set; }

	/// <summary>
	/// Reads the <c>package.json</c> file located in the specified directory.
	/// </summary>
	/// <param name="input">The path to the root directory of the Node.js project.</param>
	/// <returns>The <c>package.json</c> of the specified Node.js project, or <see langword="null"/> if not found.</returns>
	public static PackageJsonFile? ReadFromDirectory(DirectoryInfo input) {
		var path = Path.Join(input.FullName, "package.json");
		return File.Exists(path) ? JsonSerializer.Deserialize<PackageJsonFile>(File.ReadAllText(path)) : null;
	}
}

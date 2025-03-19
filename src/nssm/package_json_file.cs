namespace Belin.Cli.Nssm;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the contents of a <c>package.json</c> file.
/// </summary>
public record PackageJsonFile {

	/// <summary>
	/// The map of package commands.
	/// </summary>
	[JsonPropertyName("bin")]
	public IDictionary<string, string>? Bin { get; init; } = null;

	/// <summary>
	/// The package description.
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; init; } = null;

	/// <summary>
	/// The package name.
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; init; }

	/// <summary>
	/// The version number.
	/// </summary>
	[JsonPropertyName("version")]
	public required string Version { get; init; }

	/// <summary>
	/// Reads the <c>package.json</c> file located in the specified directory.
	/// </summary>
	/// <param name="input">The path to the root directory of the Node.js project.</param>
	/// <returns>The <c>package.json</c> of the specified Node.js project, or <see langword="null"/> if not found.</returns>
	public static PackageJsonFile? ReadFromDirectory(string input) {
		var path = Path.Join(input, "package.json");
		return File.Exists(path) ? JsonSerializer.Deserialize<PackageJsonFile>(File.ReadAllText(path)) : null;
	}
}

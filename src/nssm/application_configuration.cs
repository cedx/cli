namespace Belin.Cli.Nssm;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Provides the configuration of a Node.js application.
/// </summary>
public class ApplicationConfiguration {

	/// <summary>
	/// The application identifier.
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	/// <summary>
	/// The application name.
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// Reads the configuration file of the Node.js application located in the specified directory.
	/// </summary>
	/// <param name="input">The path to the root directory of the Node.js application.</param>
	/// <returns>The configuration of the specified Node.js application, or <see langword="null"/> if not found.</returns>
	public static ApplicationConfiguration? ReadFromDirectory(DirectoryInfo input) {
		foreach (var folder in new[] { "lib/server", "lib", "src/server", "src" }) {
			var path = Path.Join(input.FullName, folder, "config.g.json");
			if (File.Exists(path)) return JsonSerializer.Deserialize<ApplicationConfiguration>(File.ReadAllText(path));
		}

		return null;
	}
}

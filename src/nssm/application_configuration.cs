namespace Belin.Cli.Nssm;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Provides the configuration of an application.
/// </summary>
public class ApplicationConfiguration {

	/// <summary>
	/// The application description.
	/// </summary>
	public required string Description { get; set; } = string.Empty;

	/// <summary>
	/// The application identifier.
	/// </summary>
	public required string Id { get; init; }

	/// <summary>
	/// The application name.
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// The path to the application root directory.
	/// </summary>
	[JsonIgnore]
	public required string RootPath { get; set; } = string.Empty;

	/// <summary>
	/// Reads the configuration file of the application located in the specified directory.
	/// </summary>
	/// <param name="input">The path to the root directory of the application.</param>
	/// <returns>The configuration of the specified application, or <see langword="null"/> if not found.</returns>
	public static ApplicationConfiguration? ReadFromDirectory(string input) {
		foreach (var folder in new[] { "lib/server", "lib", "src/server", "src" }) {
			var path = Path.Join(input, folder, "appsettings.json");
			if (File.Exists(path)) {
				var configuration = JsonSerializer.Deserialize<ApplicationConfiguration>(File.ReadAllText(path), JsonSerializerOptions.Web);
				if (configuration is not null) configuration.RootPath = input;
				return configuration;
			}
		}

		return null;
	}
}

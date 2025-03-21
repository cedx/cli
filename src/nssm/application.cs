namespace Belin.Cli.Nssm;

using System.Text.Json;
using System.Text.Json.Serialization;
using static System.IO.Path;

/// <summary>
/// Provides information about a Web application.
/// </summary>
public class Application {

	/// <summary>
	/// The application description.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The application identifier.
	/// </summary>
	public required string Id { get; init; }

	/// <summary>
	/// The application name.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// The path to the application root directory.
	/// </summary>
	[JsonIgnore]
	public string Path { get; set; } = string.Empty;

	/// <summary>
	/// Reads the configuration file of the application located in the specified directory.
	/// </summary>
	/// <param name="input">The path to the root directory of the application.</param>
	/// <returns>The configuration of the specified application, or <see langword="null"/> if not found.</returns>
	public static Application? ReadFromDirectory(string input) {
		foreach (var folder in new[] { "src/server", "src" }) {
			var path = Join(input, folder, "appsettings.json");
			if (File.Exists(path)) {
				var application = JsonSerializer.Deserialize<Application>(File.ReadAllText(path), JsonSerializerOptions.Web);
				if (application is not null) application.Path = input;
				return application;
			}
		}

		return null;
	}
}

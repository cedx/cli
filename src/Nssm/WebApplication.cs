namespace Belin.Cli.Nssm;

using namespace System.Text.Json;
using namespace System.Text.Json.Serialization;
using namespace System.Xml;
using namespace System.Xml.Serialization;
using static System.IO.Path;

/// <summary>
/// Provides information about a Web application.
/// </summary>
[XmlRoot("configuration")]
class WebApplication {

	/// <summary>
	/// The application description.
	/// </summary>
	[XmlElement("description")]
	[string] Description { get; set; } = "";

	/// <summary>
	/// The environment name.
	/// </summary>
	[XmlElement("environment")]
	[string] Environment { get; set; } = "";

	/// <summary>
	/// The application identifier.
	/// </summary>
	[XmlElement("id")]
	[ValidateNotNullOrWhiteSpace()] [string] Id { get; init; }

	/// <summary>
	/// The application name.
	/// </summary>
	[XmlElement("name")]
	[string] Name { get; set; } = "";

	/// <summary>
	/// The path to the application root directory.
	/// </summary>
	[JsonIgnore]
	[string] Path { get; set; } = "";

	/// <summary>
	/// Reads the configuration file of the application located in the specified directory.
	/// </summary>
	/// <param name="input">The path to the root directory of the application.</param>
	/// <returns>The configuration of the specified application, or <see langword="null"/> if not found.</returns>
	public static WebApplication? ReadFromDirectory(string input) {
		foreach (var folder in new[] { "src/Server", "src" }) {
			foreach (var format in new[] { "json", "xml" }) {
				var path = Join(input, folder, $"appsettings.{format}");
				if (File.Exists(path)) {
					Console.WriteLine(path);
					var application = GetExtension(path) == ".xml" ? DeserializeXml(path) : DeserializeJson(path);
					if (application is not null) application.Path = input;
					return application;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Deserializes the JSON document located at the specified path.
	/// </summary>
	/// <param name="path">The path to the JSON document.</param>
	/// <returns>The deserialized application configuration.</returns>
	private static WebApplication? DeserializeJson(string path) =>
		JsonSerializer.Deserialize<WebApplication>(File.ReadAllText(path), JsonSerializerOptions.Web);

	/// <summary>
	/// Deserializes the XML document located at the specified path.
	/// </summary>
	/// <param name="path">The path to the XML document.</param>
	/// <returns>The deserialized application configuration.</returns>
	private static WebApplication? DeserializeXml(string path) {
		using var xmlReader = XmlReader.Create(path);
		return (WebApplication?) new XmlSerializer(typeof(WebApplication)).Deserialize(xmlReader);
	}
}

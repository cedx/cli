namespace Belin.Cli.Nssm;

using System.Text.Json;

/// <summary>
/// Represents the contents of a <c>package.json</c> file.
/// </summary>
/// <param name="Bin">The map of package commands.</param>
/// <param name="Description">The package description.</param>
public sealed record NodePackage(IDictionary<string, string>? Bin = null, string Description = "") {

	/// <summary>
	/// Reads the <c>package.json</c> file located in the specified directory.
	/// </summary>
	/// <param name="input">The path to the root directory of the Node.js project.</param>
	/// <returns>The <c>package.json</c> file of the specified Node.js project, or <see langword="null"/> if not found.</returns>
	public static NodePackage? ReadFromDirectory(string input) {
		var path = Path.Join(input, "package.json");
		return File.Exists(path) ? JsonSerializer.Deserialize<NodePackage>(File.ReadAllText(path), JsonSerializerOptions.Web) : null;
	}
}

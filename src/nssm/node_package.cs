namespace Belin.Cli.Nssm;

using System.Text.Json;

/// <summary>
/// Represents the contents of a <c>package.json</c> file.
/// </summary>
/// <param name="Name">The package name.</param>
/// <param name="Description">The package description.</param>
/// <param name="Bin">The map of package commands.</param>
public sealed record NodePackage(string Name, string Description = "", IDictionary<string, string>? Bin = null) {

	/// <summary>
	/// Reads the <c>package.json</c> file located in the specified directory.
	/// </summary>
	/// <param name="input">The directory path.</param>
	/// <returns>The contents of the <c>package.json</c> file, or <see langword="null"/> if not found.</returns>
	public static NodePackage? ReadFromDirectory(string input) {
		var path = Path.Join(input, "package.json");
		return File.Exists(path) ? JsonSerializer.Deserialize<NodePackage>(File.ReadAllText(path), JsonSerializerOptions.Web) : null;
	}
}

namespace Belin.Cli.CommandLine.Nssm;

using System.Text.Json;

/// <summary>
/// Provides extension methods for the NSSM commands.
/// </summary>
public static class Extensions {

	/// <summary>
	/// Reads the configuration file of the Node.js application located in the specified directory.
	/// </summary>
	/// <param name="_">The current command.</param>
	/// <param name="input">The path to the root directory of the Node.js application.</param>
	/// <returns>The configuration of the specified Node.js application.</returns>
	/// <exception cref="Exception">The configuration file could not be located.</exception>
	public static ApplicationConfiguration ReadConfiguration(this Command _, DirectoryInfo input) {
		foreach (var folder in new[] { @"lib\server", "lib", @"src\server", "src" }) {
			var path = Path.Join(input.FullName, folder, "config.g.json");
			if (File.Exists(path)) return JsonSerializer.Deserialize<ApplicationConfiguration>(File.ReadAllText(path))!;
		}

		throw new Exception("Unable to find the application configuration file.");
	}
}

/// <summary>
/// Represents the contents of a <c>package.json</c> file.
/// </summary>
internal class PackageJsonFile {
	// TODO
}

namespace Belin.Cli.Nssm;

using System.Text.Json;
using static System.IO.Path;

/// <summary>
/// Represents a Node.js application.
/// </summary>
public class NodeApplication: Application {

	/// <summary>
	/// The entry point of this application.
	/// </summary>
	/// <exception cref="EntryPointNotFoundException">The application entry point could not be resolved.</exception>
	public override string EntryPoint => entryPath.Length > 0 ? entryPath : throw new EntryPointNotFoundException("Unable to resolve the application entry point.");

	/// <summary>
	/// The name of the environment variable storing the application environment.
	/// </summary>
	public override string EnvironmentVariable => "NODE_ENV";

	/// <summary>
	/// The program used to run this application.
	/// </summary>
	public override string Program => OperatingSystem.IsWindows() ? "node.exe" : "node";

	/// <summary>
	/// The path of the application entry point.
	/// </summary>
	private readonly string entryPath = "";

	/// <summary>
	/// Creates a new Node.js application.
	/// </summary>
	/// <param name="path">The path to the application root directory.</param>
	public NodeApplication(string path): base(path) {
		var packagePath = Join(Path, "package.json");
		if (File.Exists(packagePath) && JsonSerializer.Deserialize<NodePackage>(File.ReadAllText(packagePath), JsonSerializerOptions.Web) is NodePackage package) {
			if (Manifest.Description.Length == 0) Manifest.Description = package.Description;
			if (Manifest.Name.Length == 0) Manifest.Name = package.Name;
			if (package.Bin is not null && package.Bin.Count > 0) entryPath = Join(Path, package.Bin.First().Value);
		}
	}
}

/// <summary>
/// Represents the contents of a <c>package.json</c> file.
/// </summary>
/// <param name="Name">The package name.</param>
/// <param name="Description">The package description.</param>
/// <param name="Bin">The dictionary of package commands.</param>
public record NodePackage(
	string Name = "",
	string Description = "",
	IDictionary<string, string>? Bin = null
);

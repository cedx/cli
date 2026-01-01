namespace Belin.Cli.Nssm;

using IOPath = System.IO.Path;

/// <summary>
/// Represents a web application.
/// </summary>
public abstract class Application {

	/// <summary>
	/// The entry point of this application.
	/// </summary>
	public abstract string EntryPoint { get; }

	/// <summary>
	/// The name of the environment variable storing the application environment.
	/// </summary>
	public abstract string EnvironmentVariable { get; }
	
	/// <summary>
	/// The application manifest.
	/// </summary>
	public ApplicationManifest Manifest { get; } = new();

	/// <summary>
	/// The path to the application root directory.
	/// </summary>
	public string Path { get; }

	/// <summary>
	/// The program used to run this application.
	/// </summary>
	public abstract string Program { get; }

	/// <summary>
	/// Creates a new application.
	/// </summary>
	/// <param name="path">The path to the application root directory.</param>
	/// <exception cref="EntryPointNotFoundException">The application manifest could not be located.</exception>
	protected Application(string path) {
		Path = IOPath.TrimEndingDirectorySeparator(IOPath.GetFullPath(path));

		foreach (var folder in new[] { "src/Server", "src" }) {
			foreach (var format in new[] { "json", "psd1", "xml" }) {
				var file = IOPath.Join(Path, folder, $"appsettings.{format}");
				if (!File.Exists(file)) continue;
				if (ApplicationManifest.Read(file) is ApplicationManifest manifest) Manifest = manifest;
				goto End;
			}
		}

		End:
		if (string.IsNullOrWhiteSpace(Manifest.Id)) throw new EntryPointNotFoundException("Unable to locate the application manifest.");
	}
}

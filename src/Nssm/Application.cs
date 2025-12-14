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
	public ApplicationManifest Manifest { get; set; } = new();

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
	/// <exception cref="EntryPointNotFoundException">The application configuration file could not be located.</exception>
	protected Application(string path) {
		Path = IOPath.TrimEndingDirectorySeparator(IOPath.GetFullPath(path));

		foreach (var folder in new[] { "src/Server", "src" }) {
			foreach (var format in new[] { "json", "psd1", "xml" }) {
				var file = IOPath.Join(Path, folder, $"appsettings.{format}");
				if (!File.Exists(file)) continue;

				var manifest = format switch {
					"psd1" => ReadPowerShellModule(file),
					_ => null
				};

				if (manifest is not null) Manifest = manifest;
				goto End;
			}
		}

		End:
		if (string.IsNullOrEmpty(Manifest.Id)) throw new EntryPointNotFoundException("Unable to locate the application configuration file.");
	}

	/// <summary>
	/// TODO
	/// </summary>
	/// <param name="path">The path to the PowerShell module manifest.</param>
	/// <returns>The application manifest corresponding to the PowerShell module.</returns>
	private static ApplicationManifest ReadPowerShellModule(string path) {
		var manifest = PowerShellDataFile.Read(path);
		return new() {
			Description = manifest.Description,
			Environment = manifest.TryGetValue("Environment", out var environment) ? environment as string : null,
			Id = manifest.TryGetValue("Id", out var id) ? id as string : null,
			Name = manifest.TryGetValue("Name", out var name) ? name as string : null
		};
	}
}

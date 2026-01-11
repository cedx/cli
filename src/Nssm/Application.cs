namespace Belin.Cli.Nssm;

using static System.IO.Path;

/// <summary>
/// Represents a web application.
/// </summary>
public abstract class Application {
	
	/// <summary>
	/// The list of application folders.
	/// </summary>
	private static readonly string[] folders = ["src/Server", "src"];

	/// <summary>
	/// The list of configuration formats.
	/// </summary>
	private static readonly string[] formats = ["json", "psd1", "xml"];

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
		Path = TrimEndingDirectorySeparator(GetFullPath(path));

		foreach (var folder in folders) {
			var files = formats.Select(format => Join(Path, folder, $"appsettings.{format}")).Where(File.Exists);
			foreach (var file in files) if (ApplicationManifest.Read(file) is ApplicationManifest manifest) {
				Manifest = manifest;
				goto End;
			}
		}

		End:
		if (Manifest.Id.Length == 0) throw new EntryPointNotFoundException("Unable to locate the application manifest.");
	}
}

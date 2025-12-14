namespace Belin.Cli.Nssm;

using IOPath = System.IO.Path;

/// <summary>
/// Represents a Node.js application.
/// </summary>
public class PowerShellApplication: Application {

	/// <summary>
	/// The entry point of this application.
	/// </summary>
	/// <exception cref="EntryPointNotFoundException">The application entry point could not be resolved.</exception>
	public override string EntryPoint => entryPath.Length > 0 ? entryPath : throw new EntryPointNotFoundException("Unable to resolve the application entry point.");

	/// <summary>
	/// The name of the environment variable storing the application environment.
	/// </summary>
	public override string EnvironmentVariable => "PODE_ENVIRONMENT";

	/// <summary>
	/// The program used to run this application.
	/// </summary>
	public override string Program => OperatingSystem.IsWindows() ? "pwsh.exe" : "pwsh";

	/// <summary>
	/// The path of the application entry point.
	/// </summary>
	private readonly string entryPath = "";

	/// <summary>
	/// Creates a new Node.js application.
	/// </summary>
	/// <param name="path">The path to the application root directory.</param>
	public PowerShellApplication(string path): base(path) {
		var file = Directory.EnumerateFiles(path, "*.psd1").Where(file => IOPath.GetFileNameWithoutExtension(file) != "PSModules").Single();
		var module = PowerShellDataFile.Read(file);
		if (Description.Length == 0 && module.Description is not null) Description = module.Description;
		if (Name.Length == 0) Name = IOPath.GetFileNameWithoutExtension(file);
		if (module.RootModule is not null) entryPath = IOPath.Join(path, module.RootModule);
	}
}

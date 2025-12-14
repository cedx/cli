namespace Belin.Cli.Nssm;

/// <summary>
/// Represents a .NET application.
/// </summary>
public class DotNetApplication: Application {

	/// <summary>
	/// The entry point of this application.
	/// </summary>
	/// <exception cref="EntryPointNotFoundException">The application entry point could not be resolved.</exception>
	public override string EntryPoint => entryPath.Length > 0 ? entryPath : throw new EntryPointNotFoundException("Unable to resolve the application entry point.");

	/// <summary>
	/// The name of the environment variable storing the application environment.
	/// </summary>
	public override string EnvironmentVariable => "DOTNET_ENVIRONMENT";

	/// <summary>
	/// The program used to run this application.
	/// </summary>
	public override string Program => OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet";

	/// <summary>
	/// The path of the application entry point.
	/// </summary>
	private string entryPath = "";

	/// <summary>
	/// Creates a new .NET application.
	/// </summary>
	/// <param name="path">The path to the application root directory.</param>
	public DotNetApplication(string path): base(path) {
		// TODO
	}
}

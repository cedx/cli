namespace Belin.Cli.Nssm;

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
	private string entryPath = "";

	/// <summary>
	/// Creates a new Node.js application.
	/// </summary>
	/// <param name="path">The path to the application root directory.</param>
	public NodeApplication(string path): base(path) {
		// TODO
	}
}

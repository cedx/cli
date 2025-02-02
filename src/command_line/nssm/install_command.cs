namespace Belin.Cli.CommandLine.Nssm;

/// <summary>
/// Registers the Windows service.
/// </summary>
public class InstallCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public InstallCommand(): base("install", "Register the Windows service.") {
		var workingDirectory = new DirectoryInfo(Environment.CurrentDirectory);
		Add(new Argument<DirectoryInfo>("directory", () => workingDirectory, "The path to the root directory of the Node.js application."));
		this.SetHandler(Execute);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <returns>The exit code.</returns>
	public async Task<int> Execute() {
		if (!this.CheckPrivilege()) return 1;



		return await Task.FromResult(0);
	}

	/// <summary>
	/// TODO
	/// </summary>
	/// <param name="command"></param>
	/// <returns></returns>
	private static FileInfo GetPathFromEnvironment(string command) {
		return Environment.GetEnvironmentVariable("Path")!
			.Split(";")
			.Where(path => !string.IsNullOrWhiteSpace(path))
			.Select(path => new FileInfo(Path.Join(path.Trim(), command)))
			.First(file => file.Exists);
	}
}

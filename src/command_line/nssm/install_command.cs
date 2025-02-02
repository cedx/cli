namespace Belin.Cli.CommandLine.Nssm;

/// <summary>
/// Registers the Windows service.
/// </summary>
public class InstallCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public InstallCommand(): base("install", "Register the Windows service.") {
		var workingDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
		Add(new Argument<DirectoryInfo>("directory", () => workingDirectory, "The path to the root directory of the Node.js application."));
		this.SetHandler(Execute);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <returns>The exit code.</returns>
	private async Task<int> Execute() {
		if (!this.CheckPrivilege()) return 1;
		return await Task.FromResult(0);
	}
}

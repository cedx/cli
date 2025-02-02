namespace Belin.Cli.CommandLine;

/// <summary>
/// Downloads and installs the latest Node.js release.
/// </summary>
public class NodeCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public NodeCommand(): base("node", "Download and install the latest Node.js release.") {
		var configOption = new Option<FileInfo>(["-c", "--config"], "The path to the NSSM configuration file.");
		Add(configOption);
		var outputOption = new OutputOption(new DirectoryInfo(@"C:\Program Files\Node.js"));
		Add(outputOption);
		this.SetHandler(Execute, outputOption, configOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="output">The path to the output directory.</param>
	/// <param name="config">The path to the NSSM configuration file.</param>
	/// <returns>The exit code.</returns>
	private async Task<int> Execute(DirectoryInfo output, FileInfo? config) {
		if (!this.CheckPrivilege(output)) return 1;

		return await Task.FromResult(0);
	}
}

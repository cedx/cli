namespace Belin.Cli.Commands;

/// <summary>
/// Downloads and installs the latest Node.js release.
/// </summary>
public class NodeCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public NodeCommand(): base("node", "Download and install the latest Node.js release.") {
		var configOptions = new Option<FileInfo>(["-c", "--config"], "The path to the NSSM configuration file.");
		var outputOption = new OutputOption(new DirectoryInfo(@"C:\Program Files\Node.js"));
		Add(configOptions);
		Add(outputOption);
		this.SetHandler(Execute, configOptions, outputOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <param name="output">The path to the output directory.</param>
	/// <returns>The exit code.</returns>
	private async Task<int> Execute(FileInfo config, DirectoryInfo output) {
		if (!Environment.IsPrivilegedProcess) {
			Console.WriteLine("You must run this command in an elevated prompt.");
			return 1;
		}

		return await Task.FromResult(0);
	}
}

namespace Belin.Cli.Commands;

/// <summary>
/// Downloads and installs the latest PHP release.
/// </summary>
public class PhpCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public PhpCommand(): base("php", "Download and install the latest PHP release.") {
		Add(new OutputOption(OperatingSystem.IsWindows() ? @"C:\Program Files\PHP" : "/usr/local"));
		this.SetHandler(Run);
	}

	/// <summary>
	/// Runs this command.
	/// </summary>
	private void Run() {

	}
}

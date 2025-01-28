namespace Belin.Cli.Commands;

using Belin.Cli.Commands.Nssm;

/// <summary>
/// Registers a Node.js application as a Windows service.
/// </summary>
public class NssmCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public NssmCommand(): base("nssm", "Register a Node.js application as a Windows service.") {
		Add(new InstallCommand());
		Add(new RemoveCommand());
		this.SetHandler(Execute);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	public void Execute() {

	}
}

namespace Belin.Cli;

using Belin.Cli.Nssm;

/// <summary>
/// Registers a Node.js application as a Windows service.
/// </summary>
public sealed class NssmCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public NssmCommand(): base("nssm", "Register a Node.js application as a Windows service.") {
		Add(new InstallCommand());
		Add(new RemoveCommand());
	}
}

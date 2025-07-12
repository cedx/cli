namespace Belin.Cli;

using Belin.Cli.Nssm;

/// <summary>
/// Registers a .NET or Node.js application as a Windows service.
/// </summary>
public class NssmCommand: Command {

	/// <summary>
	/// Creates a new <c>nssm</c> command.
	/// </summary>
	/// <param name="install">The <c>install</c> subcommand.</param>
	/// <param name="remove">The <c>remove</c> subcommand.</param>
	public NssmCommand(InstallCommand install, RemoveCommand remove): base("nssm", "Register a .NET or Node.js application as a Windows service.") {
		Subcommands.Add(install);
		Subcommands.Add(remove);
	}
}

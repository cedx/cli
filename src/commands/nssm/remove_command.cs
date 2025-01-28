namespace Belin.Cli.Commands.Nssm;

/// <summary>
///
/// </summary>
public class RemoveCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public RemoveCommand(): base("remove", "Unregister the Windows service.") {
		this.SetHandler(Execute);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	private void Execute() {

	}
}

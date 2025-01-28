namespace Belin.Cli.Commands;

/// <summary>
/// Converts the encoding of input files.
/// </summary>
public class IconvCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public IconvCommand(): base("iconv", "Convert the encoding of input files.") {
		this.SetHandler(Execute);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	public void Execute() {

	}
}

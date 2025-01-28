namespace Belin.Cli.Commands;

/// <summary>
/// Downloads and installs the latest PHP release.
/// </summary>
public class PhpCommand: Command {

	/// <summary>
	/// The HTTP client.
	/// </summary>
	private readonly HttpClient http = new(); // using var => IDisposable !

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public PhpCommand(): base("php", "Download and install the latest PHP release.") {
		var outputOption = new OutputOption(OperatingSystem.IsWindows() ? @"C:\Program Files\PHP" : "/usr/local");
		Add(outputOption);
		this.SetHandler(Execute, outputOption);
	}

	/// <summary>
	/// Executes this command.
	/// </summary>
	/// <exception cref="PlatformNotSupportedException">This command only supports the Windows platform.</exception>
	public void Execute(string output) {
		if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("This command only supports the Windows platform.");

		using var http = new HttpClient();
	}

	/// <summary>
	/// Fetches the latest version number of the PHP releases.
	/// </summary>
	private string FetchLatestVersion() {
		Console.WriteLine("Fetching the list of PHP releases...");


		return "TODO";
	}
}

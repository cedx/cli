namespace Belin.Cli.Commands;

using System.Text.Json;

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
	private void Execute(string output) {
		if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("This command only supports the Windows platform.");

		using var httpClient = new HttpClient();
	}

	/// <summary>
	/// Fetches the latest version number of the PHP releases.
	/// </summary>
	/// <param name="httpClient">The HTTP client.</param>
	private async Task<string> FetchLatestVersion(HttpClient httpClient) {
		Console.WriteLine("Fetching the list of PHP releases...");

		using var json = JsonDocument.Parse(await httpClient.GetStringAsync("https://www.php.net/releases/?json"));
		var property = json.RootElement.EnumerateObject().FirstOrDefault();
		Console.WriteLine(property);
		//if (property is null) throw new Exception("Unable to fetch the list of PHP releases.");

		return "TODO";
	}
}

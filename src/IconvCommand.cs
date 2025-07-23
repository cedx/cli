namespace Belin.Cli;

using System.Text;
using System.Text.Json;

/// <summary>
/// Converts the encoding of input files.
/// </summary>
public class IconvCommand: Command {

	/// <summary>
	/// The path to the file or directory to process.
	/// </summary>
	private readonly Argument<FileSystemInfo> fileOrDirectoryArgument = new("fileOrDirectory") {
		Description = "The path to the file or directory to process."
	};

	/// <summary>
	/// The input encoding.
	/// </summary>
	private readonly EncodingOption fromOption = new("--from", ["-f"], Encoding.Latin1.WebName) {
		Description = "The input encoding."
	};

	/// <summary>
	/// The output encoding.
	/// </summary>
	private readonly EncodingOption toOption = new("--to", ["-t"], Encoding.UTF8.WebName) {
		Description = "The output encoding."
	};

	/// <summary>
	/// Value indicating whether to process the directory recursively.
	/// </summary>
	private readonly Option<bool> recursiveOption = new("--recursive", ["-r"]) {
		Description = "Whether to process the directory recursively."
	};

	/// <summary>
	/// The list of binary file extensions.
	/// </summary>
	private readonly List<string> binaryExtensions = [];

	/// <summary>
	/// The list of text file extensions.
	/// </summary>
	private readonly List<string> textExtensions = [];

	/// <summary>
	/// Creates a new <c>iconv</c> command.
	/// </summary>
	/// <param name="logger">The logging service.</aparam>
	public IconvCommand(): base("iconv", "Convert the encoding of input files.") {
		Arguments.Add(fileOrDirectoryArgument);
		Options.Add(fromOption);
		Options.Add(toOption);
		Options.Add(recursiveOption);
		SetAction(InvokeAsync);
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <returns>The exit code.</returns>
	public int Invoke(ParseResult parseResult) {
		var fileOrDirectory = parseResult.GetRequiredValue(fileOrDirectoryArgument);
		if (!fileOrDirectory.Exists) {
			Console.Error.WriteLine("Unable to locate the specified file or directory.");
			return 1;
		}

		var resources = Path.Join(AppContext.BaseDirectory, "../res");
		if (binaryExtensions.Count == 0) binaryExtensions.AddRange(JsonSerializer.Deserialize<string[]>(File.ReadAllText(Path.Join(resources, "BinaryExtensions.json"))) ?? []);
		if (textExtensions.Count == 0) textExtensions.AddRange(JsonSerializer.Deserialize<string[]>(File.ReadAllText(Path.Join(resources, "TextExtensions.json"))) ?? []);

		var files = fileOrDirectory switch {
			DirectoryInfo directory => directory.EnumerateFiles("*.*", parseResult.GetValue(recursiveOption) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
			FileInfo file => [file],
			_ => []
		};

		var fromEncoding = Encoding.GetEncoding(parseResult.GetRequiredValue(fromOption));
		var toEncoding = Encoding.GetEncoding(parseResult.GetRequiredValue(toOption));
		foreach (var file in files) ConvertFileEncoding(file, fromEncoding, toEncoding);
		return 0;
	}

	/// <summary>
	/// Invokes this command.
	/// </summary>
	/// <param name="parseResult">The results of parsing the command line input.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The exit code.</returns>
	public Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken) => Task.FromResult(Invoke(parseResult));

	/// <summary>
	/// Converts the encoding of the specified file.
	/// </summary>
	/// <param name="file">The path to the file to be converted.</param>
	/// <param name="from">The input encoding.</param>
	/// <param name="to">The output encoding.</param>
	private void ConvertFileEncoding(FileInfo file, Encoding from, Encoding to) {
		var extension = file.Extension.ToLowerInvariant();
		var isBinary = extension.Length > 0 && binaryExtensions.Contains(extension[1..]);
		if (isBinary) return;

		var bytes = File.ReadAllBytes(file.FullName);
		var isText = extension.Length > 0 && textExtensions.Contains(extension[1..]);
		if (!isText && Array.IndexOf(bytes, '\0', 0, Math.Min(bytes.Length, 8_000)) > 0) return;

		Console.WriteLine("Converting: {0}", file);
		File.WriteAllBytes(file.FullName, Encoding.Convert(from, to, bytes));
	}
}

/// <summary>
/// Provides the path to an output directory.
/// </summary>
internal class EncodingOption: Option<string> {

	/// <summary>
	/// Creates a new option.
	/// </summary>
	/// <param name="name">The option name.</param>
	/// <param name="aliases">The option aliases.</param>
	/// <param name="defaultValue">The default value for the option when it is not specified on the command line.</param>
	public EncodingOption(string name, string[] aliases, string defaultValue): base(name, aliases) {
		DefaultValueFactory = _ => defaultValue;
		HelpName = "encoding";
		AcceptOnlyFromAmong([Encoding.Latin1.WebName, Encoding.UTF8.WebName]);
	}
}

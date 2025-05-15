namespace Belin.Cli;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.Text;
using System.Text.Json;

/// <summary>
/// Converts the encoding of input files.
/// </summary>
public class IconvCommand: Command {

	/// <summary>
	/// Creates a new command.
	/// </summary>
	public IconvCommand(): base("iconv", "Convert the encoding of input files.") {
		Add(new Argument<FileSystemInfo>("fileOrDirectory", "The path to the file or directory to process."));
		Add(new EncodingOption(["-f", "--from"], Encoding.Latin1.WebName, "The input encoding."));
		Add(new EncodingOption(["-t", "--to"], Encoding.UTF8.WebName, "The output encoding."));
		Add(new Option<bool>(["-r", "--recursive"], "Whether to process the directory recursively."));
	}

	/// <summary>
	/// The command handler.
	/// </summary>
	/// <param name="environment">The host environment.</param>
	/// <param name="logger">The logging service.</aparam>
	public class CommandHandler(IHostEnvironment environment, ILogger<IconvCommand> logger): ICommandHandler {

		/// <summary>
		/// The path to the file or directory to process.
		/// </summary>
		public required FileSystemInfo FileOrDirectory { get; set; }

		/// <summary>
		/// The input encoding.
		/// </summary>
		public string From { get; set; } = Encoding.Latin1.WebName;

		/// <summary>
		/// Value indicating whether to process the directory recursively.
		/// </summary>
		public bool Recursive { get; set; }

		/// <summary>
		/// The output encoding.
		/// </summary>
		public string To { get; set; } = Encoding.UTF8.WebName;

		/// <summary>
		/// The list of binary file extensions.
		/// </summary>
		private readonly List<string> binaryExtensions = [];

		/// <summary>
		/// The list of text file extensions.
		/// </summary>
		private readonly List<string> textExtensions = [];

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public int Invoke(InvocationContext context) {
			if (!FileOrDirectory.Exists) {
				logger.LogError("Unable to locate the specified file or directory.");
				return 1;
			}

			var resources = Path.GetFullPath(Path.Join(environment.ContentRootPath, "../res"));
			binaryExtensions.AddRange(JsonSerializer.Deserialize<string[]>(File.ReadAllText(Path.Join(resources, "BinaryExtensions.json"))) ?? []);
			textExtensions.AddRange(JsonSerializer.Deserialize<string[]>(File.ReadAllText(Path.Join(resources, "TextExtensions.json"))) ?? []);

			var files = FileOrDirectory switch {
				DirectoryInfo directory => directory.EnumerateFiles("*.*", Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
				FileInfo file => [file],
				_ => []
			};

			var fromEncoding = Encoding.GetEncoding(From);
			var toEncoding = Encoding.GetEncoding(To);
			foreach (var file in files) ConvertFileEncoding(file, fromEncoding, toEncoding);
			return 0;
		}

		/// <summary>
		/// Invokes this command.
		/// </summary>
		/// <param name="context">The invocation context.</param>
		/// <returns>The exit code.</returns>
		public Task<int> InvokeAsync(InvocationContext context) => Task.FromResult(Invoke(context));

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

			logger.LogInformation("Converting: {File}", file);
			File.WriteAllBytes(file.FullName, Encoding.Convert(from, to, bytes));
		}
	}
}

/// <summary>
/// Provides the path to an output directory.
/// </summary>
internal class EncodingOption: Option<string> {

	/// <summary>
	/// Creates a new option.
	/// </summary>
	/// <param name="defaultValue">The default value for the option when it is not specified on the command line.</param>
	public EncodingOption(string[] aliases, string defaultValue, string description): base(aliases, () => defaultValue, description) {
		ArgumentHelpName = "encoding";
		this.FromAmong([Encoding.Latin1.WebName, Encoding.UTF8.WebName]);
	}
}

namespace Belin.Cli.Nssm;

using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using static System.IO.Path;

/// <summary>
/// Represents a .NET application.
/// </summary>
public class DotNetApplication: Application {

	/// <summary>
	/// The entry point of this application.
	/// </summary>
	/// <exception cref="EntryPointNotFoundException">The application entry point could not be resolved.</exception>
	public override string EntryPoint => entryPath.Length > 0 ? entryPath : throw new EntryPointNotFoundException("Unable to resolve the application entry point.");

	/// <summary>
	/// The name of the environment variable storing the application environment.
	/// </summary>
	public override string EnvironmentVariable => "DOTNET_ENVIRONMENT";

	/// <summary>
	/// Value indicating whether the application uses a 32-bit process.
	/// </summary>
	public override bool Is32Bit { get; } = false;

	/// <summary>
	/// The program used to run this application.
	/// </summary>
	public override string Program => OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet";

	/// <summary>
	/// The path of the application entry point.
	/// </summary>
	private readonly string entryPath = "";

	/// <summary>
	/// Creates a new .NET application.
	/// </summary>
	/// <param name="path">The path to the application root directory.</param>
	public DotNetApplication(string path): base(path) {
		foreach (var folder in sourceFolders.Select(sourceFolder => Join(Path, sourceFolder)).Where(Directory.Exists))
			if (Directory.EnumerateFiles(folder, "*.csproj").SingleOrDefault() is string projectPath) {
				var entryPoint = (AssemblyName: "", Platforms: "", OutDir: "");
				using var xmlReader = XmlReader.Create(projectPath);

				if (new XmlSerializer(typeof(CSharpProject)).Deserialize(xmlReader) is CSharpProject project) foreach (var propertyGroup in project.PropertyGroups) {
					if (Manifest.Description.Length == 0) Manifest.Description = propertyGroup.Description;
					if (Manifest.Name.Length == 0) Manifest.Name = propertyGroup.Product;
					if (entryPoint.AssemblyName.Length == 0) entryPoint.AssemblyName = propertyGroup.AssemblyName;
					if (entryPoint.Platforms.Length == 0) entryPoint.Platforms = propertyGroup.Platforms;
					if (entryPoint.OutDir.Length == 0 && propertyGroup.OutDir.Length > 0) entryPoint.OutDir = Join(GetDirectoryName(projectPath), propertyGroup.OutDir);
				}

				if (entryPoint.AssemblyName.Length == 0) entryPoint.AssemblyName = GetFileNameWithoutExtension(projectPath);
				if (entryPoint.OutDir.Length == 0) entryPoint.OutDir = Join(Path, "bin");

				entryPath = GetFullPath(Join(entryPoint.OutDir, $"{entryPoint.AssemblyName}.dll"));
				Is32Bit = entryPoint.Platforms.Split(';').Contains("x86");
				break;
			}
	}
}

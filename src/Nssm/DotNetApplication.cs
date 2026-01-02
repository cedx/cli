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
		var projectPaths = Directory.EnumerateFiles(Join(Path, "src/Server"), "*.csproj").Concat(Directory.EnumerateFiles(Join(Path, "src"), "*.csproj"));
		if (projectPaths.SingleOrDefault() is string projectPath) {
			var entryPoint = (AssemblyName: "", OutDir: "");
			using var xmlReader = XmlReader.Create(projectPath);

			if (new XmlSerializer(typeof(CSharpProject)).Deserialize(xmlReader) is CSharpProject project) foreach (var propertyGroup in project.PropertyGroups) {
				if (Manifest.Description.Length == 0) Manifest.Description = propertyGroup.Description;
				if (Manifest.Name.Length == 0) Manifest.Name = propertyGroup.Product;
				if (entryPoint.AssemblyName.Length == 0) entryPoint.AssemblyName = propertyGroup.AssemblyName;
				if (entryPoint.OutDir.Length == 0 && propertyGroup.OutDir.Length > 0) entryPoint.OutDir = Join(GetDirectoryName(projectPath), propertyGroup.OutDir);
			}

			if (entryPoint.AssemblyName.Length == 0) entryPoint.AssemblyName = GetFileNameWithoutExtension(projectPath);
			if (entryPoint.OutDir.Length == 0) entryPoint.OutDir = Join(Path, "bin");
			entryPath = GetFullPath(Join(entryPoint.OutDir, $"{entryPoint.AssemblyName}.dll"));
		}
	}
}

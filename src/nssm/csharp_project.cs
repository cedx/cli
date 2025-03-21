namespace Belin.Cli.Nssm;

using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Represents the contents of a C# project file.
/// </summary>
/// <param name="PropertyGroup">The property groups.</param>
[XmlRoot("Project")]
public sealed record CSharpProject(CSharpPropertyGroup[] PropertyGroup) {

	/// <summary>
	/// Reads the C# project file located in the specified directory.
	/// </summary>
	/// <param name="input">The directory path.</param>
	/// <returns>The contents and the path of the C# project file, or <see langword="null"/> if not found.</returns>
	public static (CSharpProject? Project, string? Path) ReadFromDirectory(string input) {
		var path = Directory.EnumerateFiles(Path.Join(input, "src"), "*.csproj", SearchOption.AllDirectories).FirstOrDefault();
		var serializer = new XmlSerializer(typeof(CSharpProject));
		return path is null ? default : (Project: (CSharpProject?) serializer.Deserialize(XmlReader.Create(path)), Path: path);
	}
}

/// <summary>
/// Represents a group of project properties.
/// </summary>
/// <param name="AssemblyName">The assembly name.</param>
/// <param name="Description">The project description.</param>
/// <param name="OutDir">The relative path of the project assemblies.</param>
public sealed record CSharpPropertyGroup(string AssemblyName = "", string Description = "", string OutDir = "");

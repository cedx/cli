namespace Belin.Cli.Nssm;

using System.Xml;
using System.Xml.Serialization;
using static System.IO.Path;

/// <summary>
/// Represents the contents of a C# project file.
/// </summary>
[XmlRoot("Project")]
public class CSharpProject {

	/// <summary>
	/// The path to the project file.
	/// </summary>
	[XmlIgnore]
	public string Path { get; set; } = "";

	/// <summary>
	/// The property groups.
	/// </summary>
	[XmlElement("PropertyGroup")]
	public CSharpPropertyGroup[] PropertyGroups { get; set; } = [];

	/// <summary>
	/// Reads the C# project file located in the specified directory.
	/// </summary>
	/// <param name="input">The directory path.</param>
	/// <returns>The contents of the C# project file, or <see langword="null"/> if not found.</returns>
	public static CSharpProject? ReadFromDirectory(string input) {
		var path = Directory.EnumerateFiles(Join(input, "src"), "*.csproj", SearchOption.AllDirectories).SingleOrDefault();
		if (path is null) return null;

		using var xmlReader = XmlReader.Create(path);
		var project = (CSharpProject?) new XmlSerializer(typeof(CSharpProject)).Deserialize(xmlReader);
		if (project is not null) project.Path = path;
		return project;
	}
}

/// <summary>
/// Represents a group of project properties.
/// </summary>
public class CSharpPropertyGroup {

	/// <summary>
	/// The assembly name.
	/// </summary>
	public string AssemblyName { get; set; } = "";

	/// <summary>
	/// The project description.
	/// </summary>
	public string Description { get; set; } = "";

	/// <summary>
	/// The product name.
	/// </summary>
	public string Product { get; set; } = "";

	/// <summary>
	/// The relative path of the project assemblies.
	/// </summary>
	public string OutDir { get; set; } = "";
}

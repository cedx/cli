namespace Belin.Cli.Nssm;

using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Represents the contents of a C# project file.
/// </summary>
[XmlRoot("Project")]
public sealed record CSharpProject {

	/// <summary>
	/// The property groups.
	/// </summary>
	public CSharpPropertyGroup[] PropertyGroup { get; set; } = [];

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
public sealed record CSharpPropertyGroup {

	/// <summary>
	/// The assembly name.
	/// </summary>
	public string AssemblyName { get; set; } = string.Empty;

	/// <summary>
	/// The project description.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The product name.
	/// </summary>
	public string Product { get; set; } = string.Empty;

	/// <summary>
	/// The relative path of the project assemblies.
	/// </summary>
	public string OutDir { get; set; } = string.Empty;
}

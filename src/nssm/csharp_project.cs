namespace Belin.Cli.Nssm;

using System.ComponentModel;
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
	/// <param name="input">The path to the root directory of the Node.js project.</param>
	/// <returns>The C# project of the specified Node.js project, or <see langword="null"/> if not found.</returns>
	public static CSharpProject? ReadFromDirectory(string input) {
		var paths = Directory.EnumerateFiles(input, "*.csproj");
		var serializer = new XmlSerializer(typeof(CSharpProject));
		return paths.Any() ? (CSharpProject?) serializer.Deserialize(XmlReader.Create(paths.First())) : null;
	}
}

/// <summary>
/// Represents a group of project properties.
/// </summary>
/// <param name="AssemblyName">The assembly name.</param>
/// <param name="Description">The project description.</param>
/// <param name="OutDir">The relative path of the project assemblies.</param>
public sealed record CSharpPropertyGroup(string AssemblyName = "", string Description = "", string OutDir = "");

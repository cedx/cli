namespace Belin.Cli.Nssm;

using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Represents the contents of a C# project file.
/// </summary>
[XmlRoot("Project")]
public class CSharpProject {

	/// <summary>
	/// The property groups.
	/// </summary>
	[XmlElement("PropertyGroup")]
	public CSharpPropertyGroup[] PropertyGroups { get; set; } = [];
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
	/// The relative path of the project assemblies.
	/// </summary>
	public string OutDir { get; set; } = "";

	/// <summary>
	/// The project description.
	/// </summary>
	public string Product { get; set; } = "";
}

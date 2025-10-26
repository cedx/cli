namespace Belin.Cli.Nssm;

using namespace System.Xml;
using namespace System.Xml.Serialization;

/// <summary>
/// Represents the contents of a NSSM configuration file.
/// </summary>
[XmlRoot("configuration")]
public sealed class NssmConfiguration {

	/// <summary>
	/// The list of machines hosting an NSSM service.
	/// </summary>
	[XmlElement("machine")]
	public NssmMachine[] Machines { get; set; } = [];

	/// <summary>
	/// Reads the NSSM configuration file located at the specified path.
	/// </summary>
	/// <param name="input">The file path.</param>
	/// <returns>The contents of the NSSM configuration file, or <see langword="null"/> if not found.</returns>
	public static NssmConfiguration? ReadFromFile(string input) {
		if (!File.Exists(input)) return null;
		using var xmlReader = XmlReader.Create(input);
		return (NssmConfiguration?) new XmlSerializer(typeof(NssmConfiguration)).Deserialize(xmlReader);
	}
}

/// <summary>
/// Represents a machine hosting an NSSM service.
/// </summary>
public sealed class NssmMachine {

	/// <summary>
	/// The machine name.
	/// </summary>
	[XmlAttribute("name")]
	[ValidateNotNullOrWhiteSpace()] [string] Name { get; set; }

	/// <summary>
	/// The list of hosted services.
	/// </summary>
	[XmlElement("service")]
	public NssmService[] Services { get; set; } = [];
}

/// <summary>
/// Represents an NSSM service managing a Web application.
/// </summary>
public sealed class NssmService {

	/// <summary>
	/// The service identifier.
	/// </summary>
	[XmlAttribute("id")]
	[ValidateNotNullOrWhiteSpace()] [string] Id { get; set; }
}

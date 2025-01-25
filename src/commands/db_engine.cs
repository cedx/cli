namespace Belin.Cli.Commands;

/// <summary>
/// Alters the storage engine of MariaDB/MySQL tables.
/// </summary>
/// <param name="name">The command name.</param>
internal class DbEngineCommand(string name): Command(name, "Alter the storage engine of MariaDB/MySQL tables.") {

}

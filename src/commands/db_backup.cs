namespace Belin.Cli.Commands;

/// <summary>
/// Backups a set of MariaDB/MySQL tables.
/// </summary>
/// <param name="name">The command name.</param>
public class DbBackupCommand(string name): Command(name, "Backup a set of MariaDB/MySQL tables.") {

}

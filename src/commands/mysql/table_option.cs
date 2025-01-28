namespace Belin.Cli.Commands;

/// <summary>
/// Provides the name of a database table.
/// </summary>
public class TableOption(): Option<string[]>(["-t", "--table"], "The table names (requires a schema).") {}

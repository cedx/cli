namespace Belin.Cli.Commands;

/// <summary>
/// Provides the name of a database schema.
/// </summary>
public class SchemaOption(): Option<string>(["-s", "--schema"], "The schema name.") {}

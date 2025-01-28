namespace Belin.Cli.Commands.MySql;

/// <summary>
/// Provides the name of a database schema.
/// </summary>
public class SchemaOption(): Option<string>(["-s", "--schema"], "The schema name.") {}

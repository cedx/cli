using Belin.Cli.Commands;

var toto = new DbBackupCommand("db-backup");
toto.SetHandler(() => Console.WriteLine("handler"));

new RootCommand("Command line interface of Cédric Belin, full stack developer.") {
	new DbBackupCommand("db-backup"),
	new DbCharsetCommand("db-charset"),
	new DbEngineCommand("db-engine"),
	new DbOptimizeCommand("db-optimize"),
	new DbRestoreCommand("db-restore"),
	new IconvCommand("iconv"),
	new JdkCommand("jdk"),
	new NodeCommand("node"),
	new NssmCommand("nssm"),
	new PhpCommand("php")
}.Invoke(args);

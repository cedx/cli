using Belin.Cli.Commands;

// Configure the command line.
var program = new RootCommand("Command line interface of Cédric Belin, full stack developer.") {
	new IconvCommand(),
	new JdkCommand(),
	new MySqlCommand(),
	new NodeCommand(),
	new NssmCommand(),
	new PhpCommand()
};

// Start the application.
if (args.Length == 0) args = ["--help"];
return await program.InvokeAsync(args);

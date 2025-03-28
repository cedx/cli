using Belin.Cli;

// Configure the command line.
var program = new RootCommand("Command line interface of Cédric Belin, full stack developer.") {
	new IconvCommand(),
	new MySqlCommand(),
	new NssmCommand(),
	new SetupCommand()
};

// Start the application.
if (args.Length == 0) args = ["--help"];
return await program.InvokeAsync(args);

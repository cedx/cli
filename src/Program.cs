using Belin.Cli;
using Microsoft.Extensions.Hosting;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

// Configure the command line.
var rootCommand = new RootCommand("Command line interface of Cédric Belin, full stack developer.") {
	new IconvCommand(),
	new MySqlCommand(),
	new NssmCommand(),
	new SetupCommand()
};

var commandLineBuilder = new CommandLineBuilder(rootCommand)
	.UseDefaults()
	.UseHost(_ => Host.CreateDefaultBuilder(args), builder => builder
		.ConfigureServices(Container.AddServices)
		.UseContentRoot(AppContext.BaseDirectory)
		.UseCommandHandler<IconvCommand, IconvCommand.CommandHandler>()
		.UseMySqlHandlers()
		.UseNssmHandlers()
		.UseSetupHandlers());

// Start the application.
if (args.Length == 0) args = ["--help"];
return await commandLineBuilder.Build().InvokeAsync(args);

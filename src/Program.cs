using Belin.Cli;
using Microsoft.Extensions.DependencyInjection;

// Configure the dependency container.
var services = new ServiceCollection();
services.AddServices();
services.AddCommands();

// Start the application.
using var serviceProvider = services.BuildServiceProvider();
var rootCommand = serviceProvider.GetRequiredService<Belin.Cli.RootCommand>();
return await rootCommand.Parse(args.Length > 0 ? args : ["--help"]).InvokeAsync();

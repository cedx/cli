using Belin.Cli;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

// Set the text of the console title bar.
var assembly = typeof(Program).Assembly;
Console.Title = assembly.GetCustomAttribute<AssemblyProductAttribute>()!.Product;

// Configure the dependency container.
var services = new ServiceCollection();
services.AddServices();
services.AddCommands();

// Start the application.
using var serviceProvider = services.BuildServiceProvider();
var rootCommand = serviceProvider.GetRequiredService<Belin.Cli.RootCommand>();
return await rootCommand.Parse(args.Length > 0 ? args : ["--help"]).InvokeAsync();

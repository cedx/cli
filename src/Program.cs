using Belin.Cli;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

// Get information about the program.
var assembly = typeof(Program).Assembly;
var product = assembly.GetCustomAttribute<AssemblyProductAttribute>()!.Product;

// Configure the dependency container.
var services = new ServiceCollection();
services.AddServices();
services.AddCommands();

// Start the application.
Console.Title = product;
Thread.Sleep(3000);
using var serviceProvider = services.BuildServiceProvider();
var rootCommand = serviceProvider.GetRequiredService<Belin.Cli.RootCommand>();
return await rootCommand.Parse(args.Length > 0 ? args : ["--help"]).InvokeAsync();

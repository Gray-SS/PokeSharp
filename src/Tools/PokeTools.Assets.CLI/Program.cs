using Spectre.Console.Cli;
using PokeCore.DependencyInjection.Ninject;
using PokeCore.DependencyInjection.Abstractions.Extensions;
using PokeTools.Assets.CLI;
using PokeTools.Assets.CLI.Services;
using PokeTools.Assets.CLI.Commands;

var services = new NinjectServiceCollections();
services.AddTransient<ICliConsole, CliConsole>();

var app = new CommandApp(new TypeRegistrar(services));
app.Configure(x =>
{
    x.SetApplicationName("poketools");
    x.SetApplicationVersion("PokéTools v0.0.1");

    x.AddCommand<BuildCommand>("build").WithAlias("test");
});

await app.RunAsync(args);
using Spectre.Console.Cli;
using PokeCore.Common.Extensions;
using PokeCore.DependencyInjection.Ninject;
using PokeCore.DependencyInjection.Abstractions.Extensions;
using PokeTools.Assets.CLI;
using PokeTools.Assets.CLI.Services;
using PokeTools.Assets.CLI.Commands;
using PokeTools.Assets.Extensions;
using PokeCore.Logging.Extensions;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.Hosting.Abstractions;

var services = new NinjectServiceCollections();
services.AddSingleton<IApp>();
services.AddSingleton<IServiceResolver>(sc => sc);
services.AddSingleton<IServiceProvider>(sc => sc);
services.AddPokeCore();
services.AddPokeToolsAssets();

services.AddTransient<ICliConsole, CliConsole>();
services.ConfigureLogging(x => x.UseEmptyLogger());

var app = new CommandApp(new TypeRegistrar(services));
app.Configure(x =>
{
#if DEBUG
    x.PropagateExceptions();
    x.ValidateExamples();
#endif

    x.SetApplicationName("poketools");
    x.SetApplicationVersion("PokéTools v0.0.1");

    // x.AddCommand<NewCommand>("new");
    x.AddCommand<BuildCommand>("build");
    x.AddCommand<BuildManifestCommand>("build-manifest");
});

Environment.ExitCode = await app.RunAsync(args);
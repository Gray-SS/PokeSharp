using System;
using PokeCore.Hosting;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.Logging.Extensions;
using PokeCore.Common.Extensions;
using PokeEngine.Extensions;
using PokeEngine.Core.Modules.Extensions;

namespace PokeRuntime.Desktop;

public sealed class PokesharpApp : App
{
    public override string AppName => "PokéSharp Runtime";
    public override Version AppVersion => new(1, 0, 0);

    protected override void Configure(IServiceContainer services)
    {
        services.UsePokeModules();
    }

    protected override void ConfigureServices(IServiceCollections services)
    {
        services.AddPokeCore();
        services.AddPokeEngineEssentials<PokesharpEngine>();

        services.ConfigureLogging(x =>
        {
            x.AddConsoleLog();
            x.AddFileLog(logDirectory: "logs");

            x.UseContextLogger();
        });
    }
}
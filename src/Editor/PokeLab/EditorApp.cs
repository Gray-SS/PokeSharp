using Ninject;
using PokeCore.Hosting;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Modules;
using PokeEngine;

namespace PokeLab;

public sealed class EditorApp : App
{
    public override string AppName => "PokéSharp Editor";
    public override Version AppVersion => new(1, 0, 0);

    protected override void OnRun()
    {
    }

    protected override void ConfigureModules(IModuleLoader loader)
    {
        loader.RegisterModule(new PokeEngineEssentials());
        loader.RegisterModule(new EditorModule());
    }

    protected override void ConfigureLogging(LoggerSettings settings)
    {
        base.ConfigureLogging(settings);

        settings.LogLevel = LogLevel.Trace;
    }

    protected override void ConfigureServices(IKernel kernel)
    {
    }
}
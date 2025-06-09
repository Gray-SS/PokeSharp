using Ninject;
using PokeSharp.Core;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;
using PokeSharp.Engine;

namespace PokeSharp.Editor;

public sealed class EditorApp : App
{
    public override string AppName => "PokéSharp Editor";
    public override Version AppVersion => new(1, 0, 0);

    protected override void OnRun()
    {
    }

    protected override void ConfigureModules(IModuleLoader loader)
    {
        loader.RegisterModule(new EngineEssentialsModule());
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
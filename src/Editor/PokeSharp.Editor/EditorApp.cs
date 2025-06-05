using PokeSharp.Assets;
using PokeSharp.Core;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Logging.Outputs;
using PokeSharp.Core.Modules;
using PokeSharp.Entities;
using PokeSharp.Inputs;
using PokeSharp.Rendering;
using PokeSharp.ROM;
using PokeSharp.Scenes;

namespace PokeSharp.Editor;

public sealed class EditorApp : App<EditorEngine>
{
    public override string AppName => "PokéSharp Editor";
    public override Version AppVersion => new Version(1, 0, 0);

    protected override void RegisterModules(IModuleLoader loader)
    {
        loader.RegisterModule(new RomModule());
        loader.RegisterModule(new AssetsModule());
        loader.RegisterModule(new InputsModule());
        loader.RegisterModule(new RenderingModule());
        loader.RegisterModule(new ScenesModule());
        loader.RegisterModule(new EntitiesModule());

        loader.RegisterModule(new EditorModule());
    }

    protected override void ConfigureLogging(LoggerSettings settings)
    {
        base.ConfigureLogging(settings);

        settings.LogLevel = LogLevel.Trace;
        settings.AddOutput(new ConsoleLogSink());
    }
}
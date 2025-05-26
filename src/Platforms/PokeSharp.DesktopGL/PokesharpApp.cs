using PokeSharp.Core;
using PokeSharp.Core.Modules;
using PokeSharp.Assets;
using PokeSharp.Inputs;
using PokeSharp.Rendering;
using PokeSharp.ROM;
using System;
using PokeSharp.Core.Logging;
using PokeSharp.Scenes;
using PokeSharp.Entities;

namespace PokeSharp.DesktopGL;

public sealed class PokesharpApp : App<PokesharpEngine>
{
    public override string AppName => "PokéSharp Runtime";
    public override Version AppVersion => new Version(1, 0, 0);

    protected override void RegisterModules(IModuleLoader loader)
    {
        loader.RegisterModule(new RomModule());
        loader.RegisterModule(new AssetsModule());
        loader.RegisterModule(new InputsModule());
        loader.RegisterModule(new RenderingModule());
        loader.RegisterModule(new ScenesModule());
        loader.RegisterModule(new EntitiesModule());
    }

    protected override void ConfigureLogging(LoggerSettings settings)
    {
        base.ConfigureLogging(settings);

        settings.AddOutput(new ConsoleLogOutput());

#if DEBUG
        settings.LogLevel = LogLevel.Trace;
#else
        settings.LogLevel = LogLevel.Info;
#endif
    }
}
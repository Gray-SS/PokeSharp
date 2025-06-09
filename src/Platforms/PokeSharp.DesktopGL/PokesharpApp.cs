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
using PokeSharp.Core.Logging.Outputs;
using Ninject;
using PokeSharp.Engine.Core;
using PokeSharp.Engine;

namespace PokeSharp.DesktopGL;

public sealed class PokesharpApp : App
{
    public override string AppName => "PokéSharp Runtime";
    public override Version AppVersion => new(1, 0, 0);

    protected override void OnRun()
    {
        using PokesharpEngine engine = Kernel.Get<PokesharpEngine>();
        Kernel.Bind<BaseEngine>().ToConstant(engine);

        engine.Run();
    }

    protected override void ConfigureModules(IModuleLoader loader)
    {
        loader.RegisterModule(new EngineEssentialsModule());
    }

    protected override void ConfigureLogging(LoggerSettings settings)
    {
        base.ConfigureLogging(settings);

        settings.AddOutput(new ConsoleLogSink());
    }

    protected override void ConfigureServices(IKernel kernel)
    {
        kernel.Bind<PokesharpEngine>().ToSelf().InSingletonScope();
    }
}
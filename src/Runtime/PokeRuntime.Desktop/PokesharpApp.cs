using System;
using Ninject;
using PokeEngine;
using PokeEngine.Core;
using PokeCore.Hosting;
using PokeCore.Hosting.Modules;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Logging.Outputs;

namespace PokeRuntime.Desktop;

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
        loader.RegisterModule(new PokeEngineEssentials());
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
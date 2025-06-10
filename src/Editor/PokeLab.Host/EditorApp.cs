using Ninject;
using PokeEngine.Assets;
using PokeCore.Hosting;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Logging.Outputs;
using PokeCore.Hosting.Modules;
using PokeEngine.Entities;
using PokeEngine.Inputs;
using PokeEngine.Rendering;
using PokeEngine.ROM;
using PokeEngine.Scenes;

namespace PokeLab.Host;

public sealed class EditorApp : App
{
    public override string AppName => "PokéSharp Editor";
    public override Version AppVersion => new(1, 0, 0);

    protected override void ConfigureModules(IModuleLoader loader)
    {
        // TODO: - [ ] Create a centralized project that references all engine-related modules.
        //       - [ ] Create a module named "EngineModule" that loads all the required engine submodules.

        loader.RegisterModule(new AssetsModule());
        loader.RegisterModule(new RomModule());
        loader.RegisterModule(new InputsModule());
        loader.RegisterModule(new ScenesModule());
        loader.RegisterModule(new EntitiesModule());
        loader.RegisterModule(new RenderingModule());
    }

    protected override void ConfigureServices(IKernel kernel)
    {
    }

    protected override void ConfigureLogging(LoggerSettings settings)
    {
        settings.AddOutput(new ConsoleLogSink());
    }

    protected override void OnRun()
    {
    }
}
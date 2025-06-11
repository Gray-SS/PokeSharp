using Ninject;
using PokeCore.Hosting.Modules;
using PokeEngine.Assets;
using PokeEngine.Core;
using PokeEngine.Entities;
using PokeEngine.Inputs;
using PokeEngine.Rendering;
using PokeEngine.ROM;
using PokeEngine.Scenes;

namespace PokeEngine;

public class PokeEngineEssentials : Module
{
    public override string Name => "PokeEngine Essentials";

    public override void ConfigureServices(IKernel kernel)
    {
    }

    public override void RegisterSubModules(IModuleLoader loader)
    {
        loader.RegisterModule(new EngineCoreModule());
        loader.RegisterModule(new InputsModule());
        loader.RegisterModule(new EntitiesModule());
        loader.RegisterModule(new ScenesModule());
        loader.RegisterModule(new RomModule());
        loader.RegisterModule(new RenderingModule());
        loader.RegisterModule(new AssetsModule());
    }

    public override void Load()
    {

    }
}
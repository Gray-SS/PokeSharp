using Ninject;
using PokeSharp.Assets;
using PokeSharp.Core.Modules;
using PokeSharp.Engine.Core;
using PokeSharp.Entities;
using PokeSharp.Inputs;
using PokeSharp.Rendering;
using PokeSharp.ROM;
using PokeSharp.Scenes;

namespace PokeSharp.Engine;

public class EngineEssentialsModule : Module
{
    public override string Name => "PokéSharp Essentials Module";

    public override void Configure(IKernel kernel)
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
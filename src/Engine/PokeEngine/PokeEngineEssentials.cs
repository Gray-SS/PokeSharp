using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core;
using PokeEngine.Assets;
using PokeEngine.Entities;
using PokeEngine.Inputs;
using PokeEngine.Rendering;
using PokeEngine.Scenes;
using PokeEngine.Core.Modules;
using PokeEngine.Core.Modules.Extensions;

namespace PokeEngine;

public sealed class PokeEngineEssentials<TEngine> : EngineModule
    where TEngine : BaseEngine
{
    public override string Name => "PokéEngine Essentials";
    public override Version Version => new(1, 0, 0);

    public override void Configure(IServiceContainer services)
    {
    }

    public override void ConfigureServices(IServiceCollections services)
    {
        services.AddPokeModule<PokeEngineCoreModule<TEngine>>();
        services.AddPokeModule<PokeEngineAssetsModule>();
        services.AddPokeModule<PokeEngineEntitiesModule>();
        services.AddPokeModule<PokeEngineInputsModule>();
        services.AddPokeModule<PokeEngineRenderingModule>();
        services.AddPokeModule<PokeEngineScenesModule>();
    }
}
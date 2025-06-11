using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core.Modules;

namespace PokeEngine.Entities;

public sealed class PokeEngineEntitiesModule : EngineModule
{
    public override string Name => "PokéEngine Entities";
    public override Version Version => new(1, 0, 0);

    public override void Configure(IServiceContainer services)
    {
    }

    public override void ConfigureServices(IServiceCollections services)
    {
    }
}
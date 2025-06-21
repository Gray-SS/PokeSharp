using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core.Modules;

namespace PokeEngine.Scenes;

public sealed class PokeEngineScenesModule : EngineModule
{
    public override string Name => "PokéEngine Scenes";
    public override Version Version => new(1, 0, 0);

    public override void Configure(IServiceResolver services)
    {
    }

    public override void ConfigureServices(IServiceCollections services)
    {
    }
}
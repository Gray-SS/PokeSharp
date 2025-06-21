using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;
using PokeEngine.Core.Modules;

namespace PokeEngine.Rendering;

public sealed class PokeEngineRenderingModule : EngineModule
{
    public override string Name => "PokéEngine Rendering";
    public override Version Version => new(1, 0, 0);

    public override void Configure(IServiceResolver services)
    {
    }

    public override void ConfigureServices(IServiceCollections services)
    {
        services.AddSingleton<IRenderingPipeline, RenderingPipeline>();
    }
}
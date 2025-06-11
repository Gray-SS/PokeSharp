using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core.Modules;
using PokeEngine.ROM.Services;

namespace PokeEngine.ROM;

public sealed class PokeEngineRomModule : EngineModule
{
    public override string Name => "PokéEngine RomTools";
    public override Version Version => new(1, 0, 0);

    private static readonly string GBA_CONFIG_PATH = Path.Combine(AppContext.BaseDirectory, "configs", "gbas.yaml");

    public override void Configure(IServiceContainer services)
    {
    }

    public override void ConfigureServices(IServiceCollections services)
    {
        services.AddSingleton<IGbaConfigProvider>(new GbaConfigProvider(GBA_CONFIG_PATH));
    }
}
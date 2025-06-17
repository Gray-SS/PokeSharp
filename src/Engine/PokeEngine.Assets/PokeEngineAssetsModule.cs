using PokeCore.DependencyInjection.Abstractions;
using PokeCore.IO.Services;
using PokeEngine.Assets.Services;
using PokeEngine.Core.Modules;

namespace PokeEngine.Assets;

public sealed class PokeEngineAssetsModule : EngineModule
{
    public override string Name => "PokéEngine Assets";
    public override Version Version => new(1, 0, 0);

    public override void Configure(IServiceContainer services)
    {
    }

    public override void ConfigureServices(IServiceCollections services)
    {
        services.AddSingleton<AssetPipeline>();
        services.AddSingleton<IVirtualFileSystem, VirtualFileSystem>();
        services.AddSingleton<IVirtualVolumeManager, VirtualVolumeManager>();
        services.AddTransient<IAssetMetadataSerializer, AssetMetadataSerializer>();
    }
}
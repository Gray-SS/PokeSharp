using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;
using PokeTools.Assets.Services;
using PokeTools.Assets.Services.Abstractions;
using PokeTools.Assets.Pipeline.Abstractions;

namespace PokeTools.Assets.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddPokeToolsAssets(this IServiceCollections services)
    {
        services.AddSingleton<IAssetMetadataStore, DefaultMetadataStore>();
        services.AddTransient<IAssetTypeResolver, AssetTypeResolver>();
        services.AddTransient<IAssetPipelineExecutor, AssetPipelineExecutor>();
        services.AddTransient<IAssetPipelineService, AssetPipelineService>();
        services.AddTransient<IAssetPipelineProvider, AssetPipelineProvider>();

        var assembly = typeof(DependencyInjection).Assembly;
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
                continue;

            if (type.IsAssignableTo(typeof(IAssetImporter)))
            {
                services.AddTransient(type);
                services.AddTransient(typeof(IAssetImporter), x => x.GetRequiredService(type));
            }
            else if (type.IsAssignableTo(typeof(IAssetProcessor)))
            {
                services.AddTransient(type);
                services.AddTransient(typeof(IAssetProcessor), x => x.GetRequiredService(type));
            }
            else if (type.IsAssignableTo(typeof(IAssetSerializer)))
            {
                services.AddTransient(type);
                services.AddTransient(typeof(IAssetSerializer), x => x.GetRequiredService(type));
            }
        }

        return services;
    }
}
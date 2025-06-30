using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeTools.Assets.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddPokeToolsAssets(this IServiceCollections services)
    {
        services.AddTransient<IAssetPipeline, AssetPipeline>();

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
            else if (type.IsAssignableTo(typeof(IAssetCompiler)))
            {
                services.AddTransient(type);
                services.AddTransient(typeof(IAssetCompiler), x => x.GetRequiredService(type));
            }
        }

        return services;
    }
}
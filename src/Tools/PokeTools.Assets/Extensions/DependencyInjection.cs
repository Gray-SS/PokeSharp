using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeTools.Assets.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddPokeToolsAssets(this IServiceCollections services)
    {
        // TODO: - Simplifier l'interface d'injection de dépendance, peut-etre utilisé le IServiceProvider du System
        //       - Travailler sur le système d'import
        //       - Implémenter la commande d'import pour le CLI
        //       - Réfléchir à comment implémenter l'import pour des volumes virtuels.
        //         Mais se concentrer sur le système physique d'abord

        services.AddTransient<IAssetPipeline, AssetPipeline>();

        var assembly = typeof(DependencyInjection).Assembly;
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
                continue;

            if (type.IsAssignableTo(typeof(IAssetImporter)))
            {
                services.AddTransient(type);
                services.AddTransient(typeof(IAssetImporter), (sp) => sp.GetRequiredService(type));
            }
            else if (type.IsAssignableTo(typeof(IAssetProcessor)))
            {
                services.AddTransient(type);
                services.AddTransient(typeof(IAssetProcessor), (sp) => sp.GetRequiredService(type));
            }
        }

        return services;
    }
}
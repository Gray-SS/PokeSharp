using PokeLab.Application.ContentBrowser;
using PokeLab.Infrastructure.ContentBrowser;
using PokeCore.DependencyInjection.Abstractions;

namespace PokeLab.Infrastructure.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabInfrastructure(this IServiceCollections services)
    {
        services.AddSingleton<IContentBrowserCache, DefaultContentBrowserCache>();
        services.AddSingleton<IContentBrowserNavigator, DefaultContentBrowserNavigator>();
        return services;
    }
}
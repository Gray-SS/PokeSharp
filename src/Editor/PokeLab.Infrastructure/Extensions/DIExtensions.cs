using PokeLab.Application.ContentBrowser;
using PokeLab.Infrastructure.ContentBrowser;
using PokeCore.DependencyInjection.Abstractions;
using PokeLab.Application.ProjectManagement;
using PokeLab.Infrastructure.ProjectManagement;

namespace PokeLab.Infrastructure.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabInfrastructure(this IServiceCollections services)
    {
        services.AddSingleton<IContentBrowserCache, DefaultContentBrowserCache>();
        services.AddSingleton<IContentBrowserNavigator, DefaultContentBrowserNavigator>();
        services.AddTransient<IProjectRepository, FileProjectRepository>();

        return services;
    }
}
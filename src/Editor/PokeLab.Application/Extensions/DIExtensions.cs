using PokeLab.Application.ContentBrowser;
using PokeLab.Application.ProjectManagement;
using PokeLab.Application.Commands.Extensions;
using PokeLab.Application.Commands.Middlewares;
using PokeCore.DependencyInjection.Abstractions;
using PokeLab.Application.Events.Extensions;

namespace PokeLab.Application.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabApplication(this IServiceCollections services)
    {
        services.AddSingleton<IProjectService, ProjectService>();
        services.AddSingleton<IContentBrowserService, ContentBrowserService>();

        services.AddEvent();
        services.AddEventHandlers(typeof(DIExtensions).Assembly);

        services.AddCommand();
        services.AddCommandHandlers(typeof(DIExtensions).Assembly);

#if DEBUG
        // This middleware adds fake latency useful when working with the UI
        services.AddCommandMiddleware(new FakeLatencyMiddleware(latencyInMilliseconds: 1000));
#endif

        return services;
    }
}
using PokeLab.Presentation.Common;
using PokeCore.DependencyInjection.Abstractions;
using PokeLab.Application.Events.Extensions;
using PokeLab.Presentation.ContentBrowser;
using PokeLab.Presentation.MainMenu;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeLab.Presentation.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddPokeLabPresentation(this IServiceCollections services)
    {
        services.AddSingleton<IViewService, DefaultViewService>();
        services.AddSingleton<ITaskDispatcher, DefaultTaskDispatcher>();

        services.AddViewModel<MainMenuViewModel>();
        services.AddViewModel<ContentBrowserViewModel>();

        services.AddEventHandlers(typeof(DependencyInjection).Assembly);

        return services;
    }

    public static IServiceCollections AddView<TView>(this IServiceCollections services)
        where TView : class, IView
    {
        services.AddSingleton<TView>();
        services.AddSingleton<IView>(sc => sc.GetRequiredService<TView>());

        return services;
    }

    public static IServiceCollections AddViewModel<TViewModel>(this IServiceCollections services)
        where TViewModel : class, IViewModel
    {
        services.AddSingleton<TViewModel>();

        return services;
    }
}
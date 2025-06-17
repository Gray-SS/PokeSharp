using PokeLab.Presentation.Common;
using PokeCore.DependencyInjection.Abstractions;
using PokeLab.Application.Events.Extensions;
using PokeLab.Presentation.ContentBrowser;
using PokeLab.Presentation.MainMenu;

namespace PokeLab.Presentation.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabPresentation(this IServiceCollections services)
    {
        services.AddSingleton<IViewService, DefaultViewService>();
        services.AddSingleton<ITaskDispatcher, DefaultTaskDispatcher>();

        services.AddViewModel<MainMenuViewModel>();
        services.AddViewModel<ContentBrowserViewModel>();

        services.AddEventHandlers(typeof(DIExtensions).Assembly);

        return services;
    }

    public static IServiceCollections AddView<TView>(this IServiceCollections services)
        where TView : class, IView
    {
        services.AddSingleton<TView>();
        services.AddSingleton<IView>(sc => sc.GetService<TView>());

        return services;
    }

    public static IServiceCollections AddViewModel<TViewModel>(this IServiceCollections services)
        where TViewModel : class, IViewModel
    {
        services.AddSingleton<TViewModel>();

        return services;
    }
}
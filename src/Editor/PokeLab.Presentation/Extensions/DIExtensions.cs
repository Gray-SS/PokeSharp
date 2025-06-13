using PokeLab.Presentation.Common;
using PokeLab.Presentation.MainMenu;
using PokeLab.Presentation.ContentBrowser;
using PokeCore.DependencyInjection.Abstractions;

namespace PokeLab.Presentation.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabPresentation(this IServiceCollections services)
    {
        services.AddSingleton<IViewService, DefaultViewService>();
        services.AddSingleton<ITaskDispatcher, DefaultTaskDispatcher>();

        services.AddPresenter<MainMenuPresenter>();
        services.AddPresenter<ContentBrowserPresenter>();

        return services;
    }

    public static IServiceContainer ConfigurePokeLabPresentation(this IServiceContainer services)
    {
        // This is used to resolve the presenters so they can directly interact with the application
        _ = services.GetServices<IPresenter>().ToList();

        return services;
    }

    public static IServiceCollections AddView<TView, TViewImpl>(this IServiceCollections services)
        where TView : class, IView
        where TViewImpl : TView
    {
        services.AddSingleton<TView, TViewImpl>();
        services.AddSingleton<IView>(sc => sc.GetService<TView>());

        return services;
    }

    private static IServiceCollections AddPresenter<TPresenter>(this IServiceCollections services)
        where TPresenter : class, IPresenter
    {
        services.AddSingleton<TPresenter>();
        services.AddSingleton<IPresenter>(sc => sc.GetService<TPresenter>());

        return services;
    }
}
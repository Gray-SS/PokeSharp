using PokeLab.Presentation.Common;
using PokeLab.Presentation.MainMenu;
using PokeLab.Presentation.ContentBrowser;
using PokeCore.DependencyInjection.Abstractions;
using PokeLab.Presentation.States;

namespace PokeLab.Presentation.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabPresentation(this IServiceCollections services)
    {
        services.AddSingleton<IViewService, DefaultViewService>();
        services.AddSingleton<ITaskDispatcher, DefaultTaskDispatcher>();

        services.AddPresenter<ContentBrowserPresenter>();

        services.AddState<MainMenuState, MainMenuIntents, MainMenuReducer, MainMenuEffect>(new MainMenuState(string.Empty, string.Empty, null, MainMenuViewState.Idle, false));

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

    private static IServiceCollections AddState<TState, TIntent, TReducer, TEffect>(this IServiceCollections services, TState defaultState)
        where TState : class
        where TIntent : class
        where TReducer : class, IStateReducer<TState, TIntent>
        where TEffect : class, IStateEffect<TState, TIntent>
    {
        services.AddTransient<TReducer>();
        services.AddTransient<IStateReducer<TState, TIntent>>(sc => sc.GetService<TReducer>());

        services.AddTransient<TEffect>();
        services.AddTransient<IStateEffect<TState, TIntent>>(sc => sc.GetService<TEffect>());

        services.AddSingleton<IStateStore<TState, TIntent>>(sc =>
        {
            var reducer = sc.GetService<IStateReducer<TState, TIntent>>();
            var effect = sc.GetService<IStateEffect<TState, TIntent>>();

            return new StateStore<TState, TIntent>(defaultState, reducer, effect);
        });

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
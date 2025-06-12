using PokeCore.DependencyInjection.Abstractions;
using PokeLab.Presentation.Common;
using PokeLab.Presentation.ContentBrowser;

namespace PokeLab.Presentation.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabPresentation(this IServiceCollections services)
    {
        services.AddSingleton<IEditorViewManager, DefaultEditorViewManager>();

        services.AddSingleton<ContentBrowserController>();
        return services;
    }

    public static IServiceContainer ConfigurePokeLabPresentation(this IServiceContainer services)
    {
        services.GetService<ContentBrowserController>();

        return services;
    }

    public static IServiceCollections AddEditorView<TView, TViewImpl>(this IServiceCollections services)
        where TView : class, IEditorView
        where TViewImpl : TView
    {
        services.AddSingleton<TView, TViewImpl>();
        services.AddSingleton<IEditorView>(sc => sc.GetService<TView>());

        return services;
    }
}
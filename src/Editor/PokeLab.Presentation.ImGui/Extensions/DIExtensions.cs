using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Extensions;
using PokeLab.Presentation.ContentBrowser;
using PokeLab.Presentation.Extensions;
using PokeLab.Presentation.ImGui.Common;
using PokeLab.Presentation.ImGui.ContentBrowser;

namespace PokeLab.Presentation.ImGui.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabPresentationImGui(this IServiceCollections services)
    {
        services.AddEditorView<IContentBrowserView, ImGuiContentBrowserView>();

        services.AddSingleton<IGuiResourceManager, GuiResourceManager>();
        services.AddTransient<ITickSource, ImGuiTickSource>();
        services.AddPokeEngineEssentials<PokeLabEngine>();

        return services;
    }
}
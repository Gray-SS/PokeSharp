using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Extensions;
using PokeLab.Presentation.Common;
using PokeLab.Presentation.ContentBrowser;
using PokeLab.Presentation.Extensions;
using PokeLab.Presentation.ImGui.Common;
using PokeLab.Presentation.ImGui.ContentBrowser;
using PokeLab.Presentation.ImGui.MainMenu;
using PokeLab.Presentation.MainMenu;

namespace PokeLab.Presentation.ImGui.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabPresentationImGui(this IServiceCollections services)
    {
        // Presentation layer abstraction implementation
        services.AddView<IMainMenuView, MainMenuView>();
        services.AddView<IContentBrowserView, ImGuiContentBrowserView>();

        services.AddTransient<ITickSource, ImGuiTickSource>();
        services.AddSingleton<IWindowService, ImGuiWindowService>();

        // Custom services
        services.AddSingleton<IGuiResourceManager, GuiResourceManager>();
        services.AddPokeEngineEssentials<PokeLabEngine>();

        return services;
    }
}
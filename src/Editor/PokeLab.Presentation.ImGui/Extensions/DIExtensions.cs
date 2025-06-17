using MonoGame.ImGuiNet;
using PokeEngine.Extensions;
using PokeLab.Presentation.Common;
using PokeLab.Presentation.Extensions;
using PokeLab.Presentation.ImGui.Common;
using PokeLab.Presentation.ImGui.MainMenu;
using PokeLab.Presentation.ImGui.ContentBrowser;
using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core;

namespace PokeLab.Presentation.ImGui.Extensions;

public static class DIExtensions
{
    public static IServiceCollections AddPokeLabPresentationImGui(this IServiceCollections services)
    {
        // Presentation layer abstraction implementation
        services.AddView<MainMenuView>();
        services.AddView<ContentBrowserView>();

        services.AddTransient<ITickSource, ImGuiTickSource>();
        services.AddSingleton<IWindowService, ImGuiWindowService>();

        // Custom services
        services.AddSingleton(sc => new ImGuiRenderer(sc.GetService<BaseEngine>()));
        services.AddSingleton<IGuiResourceManager, GuiResourceManager>();
        services.AddPokeEngineEssentials<PokeLabEngine>();

        return services;
    }
}
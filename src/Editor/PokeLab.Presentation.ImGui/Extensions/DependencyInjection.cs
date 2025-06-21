using MonoGame.ImGuiNet;
using PokeEngine.Extensions;
using PokeLab.Presentation.Common;
using PokeLab.Presentation.Extensions;
using PokeLab.Presentation.ImGui.Common;
using PokeLab.Presentation.ImGui.MainMenu;
using PokeLab.Presentation.ImGui.ContentBrowser;
using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeLab.Presentation.ImGui.Extensions;

public static class DependencyInjection
{
    public static IServiceCollections AddPokeLabPresentationImGui(this IServiceCollections services)
    {
        // Presentation layer abstraction implementation
        services.AddView<MainMenuView>();
        services.AddView<ContentBrowserView>();

        services.AddTransient<ITickSource, ImGuiTickSource>();
        services.AddSingleton<IWindowService, ImGuiWindowService>();

        // Custom services
        services.AddSingleton(sc => new ImGuiRenderer(sc.GetRequiredService<BaseEngine>()));
        services.AddSingleton<IGuiResourceManager, GuiResourceManager>();
        services.AddPokeEngineEssentials<PokeLabEngine>();

        return services;
    }
}
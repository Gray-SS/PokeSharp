using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGuiNet;
using PokeEngine.Rendering;
using PokeLab.Presentation.Common;
using PokeLab.Presentation.ImGui.Common;

namespace PokeLab.Presentation.ImGui;

public sealed class PokeLabImGuiRenderer : IRenderer
{
    private static readonly ushort[] IconRanges =
    [
        0xF000, 0xF8FF, 0
    ];

    private string _projectName = string.Empty;
    private string _projectPath = string.Empty;
    private string? _formError = null;
    private bool _showCreateProjectPopup = false;

    private readonly IViewService _viewManager;
    private readonly IGuiResourceManager _resManager;
    private readonly ImGuiRenderer _imGuiRenderer;

    public PokeLabImGuiRenderer(
        ImGuiRenderer renderer,
        IGuiResourceManager resManager,
        IViewService viewManager)
    {
        _viewManager = viewManager;
        _imGuiRenderer = renderer;
        _resManager = resManager;

        ConfigureImGui();
    }

    private unsafe void ConfigureImGui()
    {
        ImGuiIOPtr io = Gui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        ImFontConfigPtr config = ImGuiNative.ImFontConfig_ImFontConfig();
        config.MergeMode = false;
        config.PixelSnapH = true;
        string interPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts", "inter_medium.ttf");
        ImFontPtr fontMedium = io.Fonts.AddFontFromFileTTF(interPath, 18.0f, config);
        if (fontMedium.NativePtr == null)
            throw new InvalidOperationException($"Couldn't load inter font at path '{interPath}'");

        _resManager.RegisterFont("medium", fontMedium);
        ImGuiNative.ImFontConfig_destroy(config.NativePtr);

        ImFontConfigPtr iconConfig = ImGuiNative.ImFontConfig_ImFontConfig();
        iconConfig.MergeMode = true;
        iconConfig.PixelSnapH = true;
        fixed (ushort* rangesPtr = IconRanges)
        {
            string faPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts", "fa-solid-900.ttf");
            ImFontPtr fa = io.Fonts.AddFontFromFileTTF(faPath, 18.0f, iconConfig, (IntPtr)rangesPtr);
            if (fa.NativePtr == null)
                throw new InvalidOperationException($"Couldn't load font awesome icons at path '{faPath}'");
        }

        ImGuiNative.ImFontConfig_destroy(iconConfig.NativePtr);

        _imGuiRenderer.RebuildFontAtlas();
    }

    public void Draw(GameTime gameTime)
    {
        _imGuiRenderer.BeforeLayout(gameTime);

        _viewManager.RenderViews();

        _imGuiRenderer.AfterLayout();
    }
}
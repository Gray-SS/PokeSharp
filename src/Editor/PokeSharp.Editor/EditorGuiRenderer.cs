using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGuiNet;
using PokeSharp.Editor.Services;
using PokeSharp.Rendering;

namespace PokeSharp.Editor;

public sealed class EditorGuiRenderer : IRenderer
{
    private readonly IGuiHookDispatcher _dispatcher;
    private readonly IGuiResourceManager _resManager;
    private readonly ImGuiRenderer _imGuiRenderer;

    public EditorGuiRenderer(ImGuiRenderer renderer, IGuiResourceManager resManager, IGuiHookDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _imGuiRenderer = renderer;
        _resManager = resManager;

        ConfigureImGui();
    }

    private unsafe void ConfigureImGui()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        // io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

        ImFontConfigPtr config = ImGuiNative.ImFontConfig_ImFontConfig();
        config.MergeMode = false;
        config.PixelSnapH = true;

        string fontPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts", "inter_medium.ttf");
        ImFontPtr fontMedium = io.Fonts.AddFontFromFileTTF(fontPath, 18.0f, config);
        _resManager.RegisterFont("medium", fontMedium);

        ImFontConfigPtr iconConfig = ImGuiNative.ImFontConfig_ImFontConfig();
        iconConfig.MergeMode = true;
        iconConfig.PixelSnapH = true;

        ushort[] iconRanges = [0xf000, 0xf8ff, 0];
        fixed (ushort* rangesPtr = iconRanges)
        {
            fontPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts", "fa-solid-900.ttf");
            io.Fonts.AddFontFromFileTTF(fontPath, 18.0f, iconConfig, (IntPtr)rangesPtr);
        }

        _imGuiRenderer.RebuildFontAtlas();

        ImGuiNative.ImFontConfig_destroy(config.NativePtr);
        ImGuiNative.ImFontConfig_destroy(iconConfig.NativePtr);
    }

    public void Draw(GameTime gameTime)
    {
        _imGuiRenderer.BeginLayout(gameTime);

        // ImGui.ShowStyleEditor();

        ImFontPtr fontMedium = _resManager.GetFont("medium");
        ImGui.PushFont(fontMedium);
        _dispatcher.Draw();
        ImGui.PopFont();

        _imGuiRenderer.EndLayout();
    }
}
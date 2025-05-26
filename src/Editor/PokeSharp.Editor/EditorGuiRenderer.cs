using ImGuiNET;
using Microsoft.Xna.Framework;
using PokeSharp.Core;
using PokeSharp.Editor.Services;
using PokeSharp.Rendering;

namespace PokeSharp.Editor;

public sealed class EditorGuiRenderer : IRenderer
{
    private readonly IGuiHookDispatcher _dispatcher;
    private readonly MonoGame.ImGuiNet.ImGuiRenderer _imGuiRenderer;

    public EditorGuiRenderer(Engine engine, IGuiHookDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _imGuiRenderer = new MonoGame.ImGuiNet.ImGuiRenderer(engine);

        ConfigureImGui();
    }

    private unsafe void ConfigureImGui()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

        ImFontConfig* nativeConfig = ImGuiNative.ImFontConfig_ImFontConfig();
        ImFontConfigPtr ptr = new ImFontConfigPtr(nativeConfig);
        ptr.SizePixels = 16.0f;

        io.Fonts.AddFontDefault(ptr);

        _imGuiRenderer.RebuildFontAtlas();

        ImGuiNative.ImFontConfig_destroy(ptr.NativePtr);
    }

    public void Draw(GameTime gameTime)
    {
        _imGuiRenderer.BeginLayout(gameTime);
        _dispatcher.Draw();
        _imGuiRenderer.EndLayout();
    }
}
using ImGuiNET;
using Microsoft.Xna.Framework;
using PokeSharp.Core;
using PokeSharp.Editor.UI.Services;
using PokeSharp.Rendering;

namespace PokeSharp.Editor.UI;

public sealed class ImGuiRenderer : IRenderer
{
    private readonly IGuiHookDispatcher _dispatcher;
    private readonly MonoGame.ImGuiNet.ImGuiRenderer _imGuiRenderer;

    public ImGuiRenderer(Engine engine, IGuiHookDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _imGuiRenderer = new MonoGame.ImGuiNet.ImGuiRenderer(engine);

        ConfigureImGui();
    }

    private unsafe void ConfigureImGui()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        ImFontConfig* nativeConfig = ImGuiNative.ImFontConfig_ImFontConfig();
        ImFontConfigPtr ptr = new ImFontConfigPtr(nativeConfig);
        ptr.SizePixels = 18.0f;

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
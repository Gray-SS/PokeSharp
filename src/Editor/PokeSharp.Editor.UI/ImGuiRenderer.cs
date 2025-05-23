using ImGuiNET;
using Microsoft.Xna.Framework;
using PokeSharp.Core;
using PokeSharp.Editor.UI.Services;
using PokeSharp.Rendering;

using NVec2 = System.Numerics.Vector2;

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

        DrawDockspace();
        _dispatcher.Draw();

        _imGuiRenderer.EndLayout();
    }

    private static void DrawDockspace()
    {
        ImGuiViewportPtr viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        ImGui.SetNextWindowViewport(viewport.ID);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, NVec2.Zero);

        ImGuiWindowFlags windowFlags = ImGuiWindowFlags.MenuBar |
                                        ImGuiWindowFlags.NoDocking |
                                        ImGuiWindowFlags.NoTitleBar |
                                        ImGuiWindowFlags.NoCollapse |
                                        ImGuiWindowFlags.NoResize |
                                        ImGuiWindowFlags.NoMove |
                                        ImGuiWindowFlags.NoBringToFrontOnFocus |
                                        ImGuiWindowFlags.NoNavFocus;

        ImGui.Begin("DockSpace Root", windowFlags);
        ImGui.PopStyleVar(3);

        uint dockspaceId = ImGui.GetID("MyDockspace");
        ImGui.DockSpace(dockspaceId, NVec2.Zero, ImGuiDockNodeFlags.None);

        ImGui.End();
    }
}
using Microsoft.Xna.Framework;
using PokeSharp.Core;
using PokeSharp.Core.Services;
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
        _imGuiRenderer.RebuildFontAtlas();
    }

    public void Draw(GameTime gameTime)
    {
        _imGuiRenderer.BeginLayout(gameTime);
        _dispatcher.Draw();
        _imGuiRenderer.EndLayout();
    }
}
using Microsoft.Xna.Framework;
using PokeSharp.Core;
using PokeSharp.Core.Services;
using PokeSharp.Rendering;

namespace PokeSharp.Editor.UI;

public sealed class ImGuiRenderer : IRenderer
{
    private readonly IImGuiHook[] _hooks;
    private readonly MonoGame.ImGuiNet.ImGuiRenderer _imGuiRenderer;

    public ImGuiRenderer(Engine engine, ReflectionManager reflectionManager)
    {
        _hooks = reflectionManager.InstantiateClassesOfType<IImGuiHook>();
        _imGuiRenderer = new MonoGame.ImGuiNet.ImGuiRenderer(engine);
        _imGuiRenderer.RebuildFontAtlas();
    }

    public void Render(GameTime gameTime)
    {
        _imGuiRenderer.BeginLayout(gameTime);
        foreach (IImGuiHook hook in _hooks)
        {
            hook.DrawGui();
        }
        _imGuiRenderer.EndLayout();
    }
}

using Microsoft.Xna.Framework;
using PokeSharp.Core;
using PokeSharp.Editor.UI;

namespace PokeSharp.Editor;

public sealed class EditorHook : IEngineHook
{
    private readonly ImGuiRenderer _imGuiRenderer;

    public EditorHook(ImGuiRenderer renderer)
    {
        _imGuiRenderer = renderer;
    }

    public void Initialize()
    {
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(GameTime gameTime)
    {
        _imGuiRenderer.Draw(gameTime);
    }
}

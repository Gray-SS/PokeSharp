using Microsoft.Xna.Framework;
using PokeSharp.Core;
using PokeSharp.Core.Attributes;
using PokeSharp.Editor.UI;

namespace PokeSharp.Editor;

[Priority(-999)]
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
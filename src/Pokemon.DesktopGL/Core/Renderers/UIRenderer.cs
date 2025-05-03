using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.Core.Graphics;

namespace Pokemon.DesktopGL.Core.Renderers;

public sealed class UIRenderer
{
    private readonly SpriteBatch _spriteBatch;

    public UIRenderer(GraphicsDevice device)
    {
        _spriteBatch = new SpriteBatch(device);
    }

    public void Begin()
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    }

    public void Draw(Sprite sprite, Rectangle bounds, Color color, float opacity = 1.0f)
    {
        _spriteBatch.Draw(sprite.Texture, bounds, sprite.SourceRect, color * opacity, 0.0f, Vector2.Zero, 0, 0.0f);
    }

    public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color)
    {
        _spriteBatch.DrawString(font, text, position, color);
    }

    public void End()
    {
        _spriteBatch.End();
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.DesktopGL.Core.Renderers;

public sealed class GameRenderer
{
    private readonly SpriteBatch _spriteBatch;

    public GameRenderer(GraphicsDevice device)
    {
        _spriteBatch = new SpriteBatch(device);
    }

    public void Begin(Camera camera)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.TransformMatrix);
    }

    public void Draw(Sprite sprite, Vector2 position, Vector2 size, Color color)
    {
        var origin = sprite.Bounds.Size.ToVector2() * 0.5f;
        var scale = new Vector2(size.X / sprite.Width, size.Y / sprite.Height);

        _spriteBatch.Draw(
            sprite.Texture,
            position,
            sprite.SourceRect,
            color,
            0.0f,
            origin,
            scale,
            SpriteEffects.None,
            0.0f
        );
    }

    public void Draw(Sprite sprite, Vector2 position, float scale, Color color)
    {
        var origin = sprite.Bounds.Size.ToVector2() * 0.5f;

        _spriteBatch.Draw(
            sprite.Texture,
            position,
            sprite.SourceRect,
            color,
            0.0f,
            origin,
            scale,
            SpriteEffects.None,
            0.0f
        );
    }

    public void Draw(Sprite sprite, Rectangle rectangle, Color color)
    {
        _spriteBatch.Draw(
            sprite.Texture,
            rectangle,
            sprite.SourceRect,
            color,
            0.0f,
            sprite.Bounds.Size.ToVector2() * 0.5f,
            0,
            0.0f
        );
    }

    public void End()
    {
        _spriteBatch.End();
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.Core.Graphics;

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

    public void DrawRect(Rectangle bounds, Color color)
    {
        var assets = PokemonGame.Instance.AssetsManager;

        _spriteBatch.Draw(
            assets.Sprite_Blank.Texture,
            bounds,
            null,
            color,
            0.0f,
            new Vector2(0.5f),
            0,
            0.0f
        );
    }

    public void Draw(Texture2D texture, Rectangle destRect, Rectangle sourceRect, Color color)
    {
        var origin = sourceRect.Size.ToVector2() * 0.5f;

        _spriteBatch.Draw(
            texture,
            destRect,
            sourceRect,
            color,
            0.0f,
            origin,
            0,
            0.0f
        );
    }

    public void End()
    {
        _spriteBatch.End();
    }
}
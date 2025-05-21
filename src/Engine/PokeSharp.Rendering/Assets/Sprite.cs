using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PokeSharp.Rendering.Assets;

public class Sprite
{
    public int Width => SourceRect.HasValue ? SourceRect.Value.Width : Texture.Width;
    public int Height => SourceRect.HasValue ? SourceRect.Value.Height : Texture.Height;
    public Texture2D Texture { get; }
    public Rectangle? SourceRect { get; }
    public Rectangle Bounds
    {
        get
        {
            int x = SourceRect.HasValue ? SourceRect.Value.X : 0;
            int y = SourceRect.HasValue ? SourceRect.Value.Y : 0;

            return new Rectangle(
                x,
                y,
                Width,
                Height
            );
        }
    }

    public Sprite(Texture2D texture, Rectangle? sourceRect = null)
    {
        Texture = texture;
        SourceRect = sourceRect;
    }

    public static Sprite FromTexture(Texture2D texture)
    {
        return new Sprite(texture);
    }
}
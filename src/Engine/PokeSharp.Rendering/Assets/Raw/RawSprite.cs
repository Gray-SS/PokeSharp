using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PokeSharp.Rendering.Assets.Raw;

public sealed class RawSprite
{
    public Texture2D Texture { get; }
    public Rectangle? SourceRect { get; }

    public RawSprite(Texture2D texture, Rectangle? sourceRect)
    {
        Texture = texture;
        SourceRect = sourceRect;
    }
}
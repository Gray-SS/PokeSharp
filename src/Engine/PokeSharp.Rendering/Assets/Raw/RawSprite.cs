using Microsoft.Xna.Framework;

namespace PokeSharp.Rendering.Assets.Raw;

public sealed class RawSprite
{
    public Rectangle? SourceRect { get; }

    public RawSprite(Rectangle? sourceRect)
    {
        SourceRect = sourceRect;
    }
}
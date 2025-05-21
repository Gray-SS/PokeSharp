using PokeSharp.Assets;
using Microsoft.Xna.Framework;

namespace PokeSharp.Rendering.Assets.Raw;

public sealed class RawSprite
{
    public AssetId TextureId { get; }
    public Rectangle? SourceRect { get; }

    public RawSprite(AssetId textureId, Rectangle? sourceRect)
    {
        TextureId = textureId;
        SourceRect = sourceRect;
    }
}
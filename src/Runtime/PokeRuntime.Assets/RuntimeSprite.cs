using System.Drawing;
using PokeCore.Assets;

namespace PokeRuntime.Assets;

public sealed class RuntimeSprite : SpriteAsset, IRuntimeAsset
{
    public RuntimeTexture? Texture { get; }

    public RuntimeSprite(Guid id, RuntimeTexture? texture, Rectangle? textureRegion) : base(id, texture?.Id, textureRegion)
    {
        Texture = texture;
    }
}
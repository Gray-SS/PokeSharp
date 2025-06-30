using System.Drawing;
using PokeCore.Assets;

namespace PokeRuntime.Assets;

public sealed class RuntimeSprite : Sprite, IRuntimeAsset
{
    public RuntimeTexture? Texture { get; }

    public RuntimeSprite(Guid id, RuntimeTexture? texture, Rectangle? sourceRectangle) : base(id, texture?.Id, sourceRectangle)
    {
    }
}
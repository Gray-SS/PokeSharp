using PokeSharp.Assets;
using PokeSharp.Rendering.Assets.Raw;

namespace PokeSharp.Rendering.Assets;

public sealed class SpriteProcessor : AssetProcessor<RawSprite, Sprite>
{
    public override Sprite Process(RawSprite rawAsset)
    {
        return new Sprite(rawAsset.Texture, rawAsset.SourceRect);
    }
}
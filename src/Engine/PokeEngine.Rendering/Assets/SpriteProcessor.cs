using PokeEngine.Assets;
using PokeEngine.Rendering.Assets.Raw;

namespace PokeEngine.Rendering.Assets;

public sealed class SpriteProcessor : AssetProcessor<RawSprite, Sprite>
{
    public override Sprite Process(RawSprite rawAsset)
    {
        return new Sprite(rawAsset.Texture, rawAsset.SourceRect);
    }
}
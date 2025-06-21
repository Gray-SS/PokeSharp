using PokeCore.Common;
using PokeEngine.Rendering;
using PokeTools.Assets.Raw;

namespace PokeTools.Assets.Processors;

public sealed class SpriteProcessor : AssetProcessor<RawSprite, Sprite>
{
    public override AssetType AssetType => AssetType.Sprite;

    public override Result<Sprite> Process(RawSprite rawAsset)
    {
        return new Sprite(rawAsset.Texture, rawAsset.SourceRect);
    }
}
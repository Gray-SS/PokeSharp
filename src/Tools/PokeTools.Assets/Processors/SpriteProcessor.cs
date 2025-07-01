using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Intermediate;

namespace PokeTools.Assets.Processors;

[AssetProcessor(AssetType.Sprite, "Sprite Processor")]
public sealed class SpriteProcessor : AssetProcessor<RawSprite, SpriteAsset>
{
    public override Result<SpriteAsset> Process(Guid assetId, RawSprite rawSprite)
    {
        return Result<SpriteAsset>.Success(new SpriteAsset(
            assetId,
            rawSprite.TextureId,
            rawSprite.TextureRegion
        ));
    }
}
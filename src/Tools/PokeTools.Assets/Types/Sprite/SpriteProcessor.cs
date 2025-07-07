using PokeCore.Assets;
using PokeCore.Common;
using PokeCore.Common.Results;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Pipeline.Processors;

namespace PokeTools.Assets.Types.Sprite;

[AssetProcessor(AssetType.Sprite, "Sprite Processor")]
public sealed class SpriteProcessor : AssetProcessor<RawSprite, CompiledSprite>
{
    public override Result<CompiledSprite> Process(Guid assetId, RawSprite rawSprite)
    {
        return new CompiledSprite(
            assetId,
            rawSprite.TextureId,
            rawSprite.TextureRegion
        );
    }
}
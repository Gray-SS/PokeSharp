using PokeCore.Common;
using PokeTools.Assets.Objects.Processed;
using PokeTools.Assets.Objects.Raw;

namespace PokeTools.Assets.Processors;

public sealed class TextureProcessor : AssetProcessor<RawTexture, ProcessedTexture>
{
    public override Result<ProcessedTexture> Process(RawTexture rawAsset)
    {
        return new ProcessedTexture(rawAsset.Width, rawAsset.Height, rawAsset.Data);
    }
}
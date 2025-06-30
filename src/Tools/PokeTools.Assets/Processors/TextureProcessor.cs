using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Intermediate;

namespace PokeTools.Assets.Processors;

[AssetProcessor(AssetType.Texture, "Texture Processor")]
public sealed class TextureProcessor : AssetProcessor<RawTexture, TextureAsset>
{
    public override Result<TextureAsset> Process(Guid assetId, RawTexture rawTexture)
    {
        if (rawTexture.Width <= 0 || rawTexture.Height <= 0)
            return Result<TextureAsset>.Failure(new Error("Processed texture dimension must be bigger than 0."));

        if (rawTexture.Data == null || rawTexture.Data.Length == 0)
            return Result<TextureAsset>.Failure(new Error("Processed texture data is invalid or empty."));

        return Result<TextureAsset>.Success(new TextureAsset(assetId, rawTexture.Width, rawTexture.Height, rawTexture.Data));
    }
}
using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.External.Annotations;
using PokeTools.Assets.External.Intermediate;

namespace PokeTools.Assets.External.Processors;

[AssetProcessor(AssetType.Texture, "Texture Processor")]
public sealed class TextureProcessor : AssetProcessor<RawTexture, Texture>
{
    public override Result<Texture> Process(Guid assetId, RawTexture rawTexture)
    {
        if (rawTexture.Width <= 0 || rawTexture.Height <= 0)
            return Result<Texture>.Failure(new Error("Processed texture dimension must be bigger than 0."));

        if (rawTexture.Data == null || rawTexture.Data.Length == 0)
            return Result<Texture>.Failure(new Error("Processed texture data is invalid or empty."));

        return Result<Texture>.Success(new Texture(assetId, rawTexture.Width, rawTexture.Height, rawTexture.Data));
    }
}
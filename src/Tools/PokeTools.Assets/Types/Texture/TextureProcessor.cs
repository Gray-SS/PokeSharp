using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Pipeline.Processors;

namespace PokeTools.Assets.Types.Texture;

[AssetProcessor(AssetType.Texture, "Texture Processor")]
public sealed class TextureProcessor : AssetProcessor<RawTexture, CompiledTexture>
{
    public override Result<CompiledTexture> Process(Guid assetId, RawTexture rawTexture)
    {
        if (rawTexture.Width <= 0 || rawTexture.Height <= 0)
            return Result.Failure("Processed texture dimension must be bigger than 0.");

        if (rawTexture.Data == null || rawTexture.Data.Length == 0)
            return Result.Failure("Processed texture data is invalid or empty.");

        return new CompiledTexture(assetId, rawTexture.Width, rawTexture.Height, rawTexture.Data);
    }
}
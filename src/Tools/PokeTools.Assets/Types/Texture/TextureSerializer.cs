using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Pipeline.Serializers;

namespace PokeTools.Assets.Types.Texture;

[AssetSerializer(AssetType.Texture, "Texture Serializer")]
public sealed class TextureSerializer : AssetSerializer<CompiledTexture>
{
    public override Result<Unit> Serialize(CompiledTexture texture, BinaryWriter writer)
    {
        if (texture.Width <= 0 || texture.Height <= 0)
            return Result.Failure("Processed texture dimension must be bigger than 0.");

        if (texture.Data == null || texture.Data.Length == 0)
            return Result.Failure("Processed texture data is invalid or empty.");

        writer.Write(texture.Width);
        writer.Write(texture.Height);

        writer.Write(texture.Data.Length);
        writer.Write(texture.Data);

        return Result.Success();
    }
}
using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets.Compilers;

[AssetCompiler(AssetType.Texture, "Texture Compiler")]
public sealed class TextureCompiler : AssetCompiler<TextureAsset>
{
    public override Result Compile(TextureAsset texture, BinaryWriter writer)
    {
        if (texture.Width <= 0 || texture.Height <= 0)
            return Result.Failure(new Error("Processed texture dimension must be bigger than 0."));

        if (texture.Data == null || texture.Data.Length == 0)
            return Result.Failure(new Error("Processed texture data is invalid or empty."));

        writer.Write(texture.Width);
        writer.Write(texture.Height);

        writer.Write(texture.Data.Length);
        writer.Write(texture.Data);

        return Result.Success();
    }
}
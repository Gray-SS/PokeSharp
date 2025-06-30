using System.Drawing;
using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets.Compilers;

[AssetCompiler(AssetType.Sprite, "Sprite Compiler")]
public sealed class SpriteCompiler : AssetCompiler<Sprite>
{
    public override Result Compile(Sprite asset, BinaryWriter writer)
    {
        writer.Write(asset.TextureId.HasValue);
        if (asset.TextureId.HasValue)
            writer.Write(asset.TextureId.Value.ToString());

        writer.Write(asset.TextureRegion.HasValue);
        if (asset.TextureRegion.HasValue)
        {
            Rectangle rect = asset.TextureRegion.Value;
            writer.Write(rect.X);
            writer.Write(rect.Y);
            writer.Write(rect.Width);
            writer.Write(rect.Height);
        }

        return Result.Success();
    }
}

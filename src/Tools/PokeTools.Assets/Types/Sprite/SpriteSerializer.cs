using System.Drawing;
using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Pipeline.Serializers;

namespace PokeTools.Assets.Types.Sprite;

[AssetSerializer(AssetType.Sprite, "Sprite Serializer")]
public sealed class SpriteSerializer : AssetSerializer<CompiledSprite>
{
    public override Result<Unit> Serialize(CompiledSprite asset, BinaryWriter writer)
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

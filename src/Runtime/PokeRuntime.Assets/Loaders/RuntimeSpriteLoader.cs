using System.Drawing;
using PokeCore.Assets;

namespace PokeRuntime.Assets.Loaders;

public sealed class RuntimeSpriteLoader(
    IAssetManager assetManager
) : RuntimeAssetLoader<RuntimeSprite>
{
    public override AssetType AssetType => AssetType.Sprite;

    public override RuntimeSprite Load(Guid assetId, BinaryReader reader)
    {
        bool hasTexture = reader.ReadBoolean();
        RuntimeTexture? texture = hasTexture ?
                                  (RuntimeTexture)assetManager.Load(Guid.Parse(reader.ReadString())) :
                                  null;

        Rectangle? textureRegion = reader.ReadBoolean() ?
                                   new Rectangle
                                   {
                                       X = reader.ReadInt32(),
                                       Y = reader.ReadInt32(),
                                       Width = reader.ReadInt32(),
                                       Height = reader.ReadInt32()
                                   } :
                                   null;

        return new RuntimeSprite(assetId, texture, textureRegion);
    }
}
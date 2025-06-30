using System.Drawing;

namespace PokeRuntime.Assets.Loaders;

public sealed class RuntimeSpriteLoader(
    IAssetManager assetManager
) : IRuntimeAssetLoader<RuntimeSprite>
{
    public RuntimeSprite Load(Guid assetId, BinaryReader reader)
    {
        RuntimeTexture? texture = reader.ReadBoolean() ?
                                  assetManager.Load<RuntimeTexture>(Guid.Parse(reader.ReadString())) :
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
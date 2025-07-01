using Microsoft.Xna.Framework.Graphics;
using PokeCore.Assets;

namespace PokeRuntime.Assets.Loaders;

public sealed class RuntimeTextureLoader(
    GraphicsDevice graphicsDevice
) : RuntimeAssetLoader<RuntimeTexture>
{
    public override AssetType AssetType => AssetType.Texture;

    public override RuntimeTexture Load(Guid id, BinaryReader reader)
    {
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();

        int length = reader.ReadInt32();
        byte[] data = reader.ReadBytes(length);

        return new RuntimeTexture(graphicsDevice, id, width, height, data);
    }
}
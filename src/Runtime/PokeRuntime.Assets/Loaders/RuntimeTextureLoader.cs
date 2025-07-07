using Microsoft.Xna.Framework.Graphics;
using PokeCore.Assets;
using PokeCore.Common.Results;

namespace PokeRuntime.Assets.Loaders;

public sealed class RuntimeTextureLoader(
    GraphicsDevice graphicsDevice
) : RuntimeAssetLoader<Texture2D>
{
    public override AssetType AssetType => AssetType.Texture;

    public override Result<Texture2D> Load(Guid id, BinaryReader reader)
    {
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();

        int length = reader.ReadInt32();
        byte[] data = reader.ReadBytes(length);

        var texture = new Texture2D(graphicsDevice, width, height);
        texture.SetData(data);

        return texture;
    }
}
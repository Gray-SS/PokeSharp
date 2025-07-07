using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using PokeCore.Assets;
using PokeCore.Common.Results;

namespace PokeRuntime.Assets.Loaders;

public sealed class RuntimeSpriteLoader(
    IAssetManager assetManager
) : RuntimeAssetLoader<Sprite>
{
    public override AssetType AssetType => AssetType.Sprite;

    public override Result<Sprite> Load(Guid assetId, BinaryReader reader)
    {
        bool hasTexture = reader.ReadBoolean();
        Texture2D? texture = hasTexture ?
            (Texture2D)assetManager.Load(Guid.Parse(reader.ReadString())) :
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

        return new Sprite(texture, textureRegion);
    }
}
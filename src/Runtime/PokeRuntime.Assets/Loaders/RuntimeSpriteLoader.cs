using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeRuntime.Assets.Extensions;

namespace PokeRuntime.Assets.Loaders;

public sealed class RuntimeSpriteLoader(
    IAssetManager assetManager
) : RuntimeAssetLoader<Sprite>
{
    public override AssetType AssetType => AssetType.Sprite;

    public override Result<Sprite> Load(Guid assetId, BinaryReader reader)
    {
        Texture2D? texture = null;
        if (reader.ReadBoolean())
        {
            Guid textureId = Guid.Parse(reader.ReadString());
            texture = assetManager.Load<Texture2D>(textureId);
        }

        Rectangle? textureRegion = null;
        if (reader.ReadBoolean())
        {
            textureRegion = new Rectangle
            {
                X = reader.ReadInt32(),
                Y = reader.ReadInt32(),
                Width = reader.ReadInt32(),
                Height = reader.ReadInt32()
            };
        }

        return new Sprite(texture, textureRegion);
    }
}
using System.Drawing;

namespace PokeCore.Assets;

public class CompiledSprite : IAsset
{
    public Guid Id { get; }
    public AssetType AssetType => AssetType.Sprite;

    public Guid? TextureId { get; }
    public Rectangle? TextureRegion { get; }

    public CompiledSprite(Guid id, Guid? textureId, Rectangle? sourceRectangle)
    {
        Id = id;
        TextureId = textureId;
        TextureRegion = sourceRectangle;
    }
}
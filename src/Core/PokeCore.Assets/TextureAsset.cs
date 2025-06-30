namespace PokeCore.Assets;

public class TextureAsset : IAsset
{
    public Guid Id { get; }
    public AssetType AssetType => AssetType.Texture;

    public int Width { get; }
    public int Height { get; }
    public byte[] Data { get; }

    public TextureAsset(Guid id, int width, int height, byte[] data)
    {
        Id = id;
        Width = width;
        Height = height;
        Data = data;
    }
}
namespace PokeTools.Assets.Intermediate;

public sealed class RawTexture : IRawAsset
{
    public int Width { get; set; }
    public int Height { get; set; }
    public byte[] Data { get; set; }

    public RawTexture(int width, int height, byte[] data)
    {
        Width = width;
        Height = height;
        Data = data;
    }

    public IEnumerable<Guid> GetDependencies()
        => [];
}
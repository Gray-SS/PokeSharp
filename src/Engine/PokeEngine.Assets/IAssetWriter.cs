namespace PokeEngine.Assets;

public interface IAssetWriter
{
    Type AssetType { get; }

    void Write(object asset, string path);
}
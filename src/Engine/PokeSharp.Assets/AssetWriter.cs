
namespace PokeSharp.Assets;

public abstract class AssetWriter<T> : IAssetWriter
    where T : class
{
    public Type AssetType => typeof(T);

    public abstract void Write(T asset, string path);

    void IAssetWriter.Write(object asset, string path)
    {
    }
}
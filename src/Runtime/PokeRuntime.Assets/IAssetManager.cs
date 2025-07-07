namespace PokeRuntime.Assets;

public interface IAssetManager
{
    void LoadBundle(string bundlePath);
    object Load(Guid assetId);
}
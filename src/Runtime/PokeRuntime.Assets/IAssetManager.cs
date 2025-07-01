using PokeCore.Common;

namespace PokeRuntime.Assets;

public interface IAssetManager
{
    Result LoadBundle(string bundlePath);

    IRuntimeAsset Load(Guid assetId);
}
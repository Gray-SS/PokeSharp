using PokeCore.Assets;
using PokeCore.Common;

namespace PokeRuntime.Assets;

public interface IAssetManager
{
    Result LoadBundle(string bundlePath);

    T Load<T>(Guid assetId) where T : IRuntimeAsset;
}
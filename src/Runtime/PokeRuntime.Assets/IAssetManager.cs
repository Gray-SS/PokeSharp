using PokeCore.Assets;

namespace PokeRuntime.Assets;

public interface IAssetManager
{
    T Load<T>(string assetPath) where T : IRuntimeAsset;
}
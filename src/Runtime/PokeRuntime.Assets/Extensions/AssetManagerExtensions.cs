namespace PokeRuntime.Assets.Extensions;

public static class AssetManagerExtensions
{
    public static T Load<T>(this IAssetManager assetManager, Guid assetId)
    {
        return (T)assetManager.Load(assetId);
    }
}
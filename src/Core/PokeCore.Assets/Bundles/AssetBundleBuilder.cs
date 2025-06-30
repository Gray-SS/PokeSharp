namespace PokeCore.Assets.Bundles;

public sealed class AssetBundleBuilder
{
    private long _offset;
    private readonly List<AssetBundleEntry> _entries = new();

    public void Register(IAsset asset)
    {
        _entries.Add(new AssetBundleEntry
        {
            AssetType = asset.AssetType,
            Offset = _offset,
        });
    }
}
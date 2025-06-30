namespace PokeCore.Assets.Bundles;

public struct AssetBundleEntryHeader
{
    public Guid AssetId { get; set; }
    public long Offset { get; set; }
}
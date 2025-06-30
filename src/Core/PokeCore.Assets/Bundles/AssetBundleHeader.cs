namespace PokeCore.Assets.Bundles;

public struct AssetBundleHeader
{
    public int AssetCount { get; set; }
    public AssetBundleEntryHeader[] Entries { get; set; }
}
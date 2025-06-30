namespace PokeCore.Assets.Bundles;

public sealed class AssetBundle
{
    public AssetBundleHeader Header { get; set; }
    public AssetBundleEntry[] Entries { get; set; }
}
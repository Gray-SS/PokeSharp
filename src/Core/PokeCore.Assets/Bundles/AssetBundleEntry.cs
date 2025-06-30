namespace PokeCore.Assets.Bundles;

public sealed class AssetBundleEntry
{
    public Guid AssetId { get; set; }
    public string Name { get; set; } = null!;
    public AssetType AssetType { get; set; }
    public uint Offset { get; set; }
    public uint Size { get; set; }
    public List<Guid> Dependencies { get; set; } = [];
}
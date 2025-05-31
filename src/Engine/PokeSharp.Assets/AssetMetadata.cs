using PokeSharp.Assets.VFS;

namespace PokeSharp.Assets;

public sealed class AssetMetadata
{
    public Guid Id { get; }
    public string AssetType { get; }
    public bool HasResource => Importer != null && Processor != null && ResourcePath != null;
    public IAssetImporter? Importer { get; set; }
    public IAssetProcessor? Processor { get; set; }
    public VirtualPath? ResourcePath { get; set; }
    public Dictionary<string, object> Metadata { get; set; }

    public bool IsDirty { get; }

    public AssetMetadata(Guid id, string assetType)
    {
        Id = id;
        AssetType = assetType;
        Metadata = new Dictionary<string, object>();

        IsDirty = true;
    }
}
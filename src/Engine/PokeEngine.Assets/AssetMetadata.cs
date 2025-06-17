using PokeCore.IO;

namespace PokeEngine.Assets;

public sealed class AssetMetadata
{
    public Guid Id { get; set; } = Guid.Empty;
    public Type AssetType { get; set; } = null!;
    public VirtualPath ResourcePath { get; set; } = null!;
    public IAssetImporter Importer { get; set; } = null!;
    public Dictionary<string, object> Metadata { get; set; }

    public bool IsValid => ErrorMessage == null;
    public string? ErrorMessage { get; set; }

    public AssetMetadata()
    {
        Metadata = new Dictionary<string, object>();
    }
}
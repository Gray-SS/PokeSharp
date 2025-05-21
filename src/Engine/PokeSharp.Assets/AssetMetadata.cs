namespace PokeSharp.Assets;

public sealed class AssetMetadata
{
    public string AssetType { get; }
    public Dictionary<string, object> Metadata { get; }

    public AssetMetadata()
    {
        AssetType = string.Empty;
        Metadata = new Dictionary<string, object>();
    }

    public AssetMetadata(string assetType, Dictionary<string, object> metadata)
    {
        AssetType = assetType;
        Metadata = metadata;
    }
}
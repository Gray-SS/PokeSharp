namespace PokeLab.Domain;

public sealed class AssetMetadata
{
    public Guid Id { get; set; } = Guid.Empty;
    public AssetType AssetType { get; set; }
}
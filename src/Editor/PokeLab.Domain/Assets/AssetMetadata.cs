using PokeCore.IO;
using PokeTools.Assets;

namespace PokeLab.Domain.Assets;

public sealed class AssetMetadata
{
    public Guid Id { get; set; } = Guid.Empty;
    public AssetType AssetType { get; set; }
    public VirtualPath ResourcePath { get; set; } = null!;

    public string? ErrorMessage { get; set; }
    public AssetMetadataState State { get; }
}
using PokeCore.Assets;

namespace PokeTools.Assets.Core;

public sealed class AssetMetadata
{
    public Guid Id { get; set; }
    public AssetType AssetType { get; set; }
}
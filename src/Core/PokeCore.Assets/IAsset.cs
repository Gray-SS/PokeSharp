namespace PokeCore.Assets;

public interface IAsset
{
    Guid Id { get; }
    AssetType AssetType { get; }
}
using PokeCore.IO;
using PokeLab.Domain.Assets;

namespace PokeLab.Application.Assets;

public interface IAssetMetadataStore
{
    bool Exists(VirtualPath assetPath);

    IVirtualFile GetMetadataFile(VirtualPath assetPath);
    VirtualPath GetMetadataPath(VirtualPath assetPath);

    IEnumerable<AssetMetadata> GetCachedMetadatas();

    void DeleteAll();

    void EnterBulkMode();
    void ExitBulkMode();

    void Save(VirtualPath assetPath, AssetMetadata metadata);
    AssetMetadata Load(VirtualPath assetPath);

    AssetMetadata? GetMetadata(Guid assetId);
    AssetMetadata? GetMetadata(VirtualPath assetPath);
}
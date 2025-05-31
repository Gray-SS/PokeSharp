using PokeSharp.Assets.VFS;

namespace PokeSharp.Assets.Services;

public interface IAssetMetadataStore
{
    bool Exists(VirtualPath assetPath);

    IVirtualFile GetMetadataFile(VirtualPath assetPath);
    VirtualPath GetMetadataPath(VirtualPath assetPath);

    void DeleteAll();
    void Save(VirtualPath assetPath, AssetMetadata metadata);
    AssetMetadata Load(VirtualPath assetPath);
}
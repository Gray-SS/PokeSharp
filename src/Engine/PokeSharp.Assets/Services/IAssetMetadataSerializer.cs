using PokeSharp.Assets.VFS;

namespace PokeSharp.Assets.Services;

public interface IAssetMetadataSerializer
{
    void Serialize(IVirtualFile vfile, AssetMetadata metadata);

    AssetMetadata Deserialize(IVirtualFile vfile);
}
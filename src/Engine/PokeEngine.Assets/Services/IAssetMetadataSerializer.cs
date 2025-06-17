using PokeCore.IO;

namespace PokeEngine.Assets.Services;

public interface IAssetMetadataSerializer
{
    void Serialize(IVirtualFile vfile, AssetMetadata metadata);

    AssetMetadata Deserialize(IVirtualFile vfile);
}
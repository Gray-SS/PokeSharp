using PokeCore.IO;
using PokeLab.Domain.Assets;

namespace PokeLab.Application.Assets;

public interface IAssetMetadataSerializer
{
    void Serialize(IVirtualFile file, AssetMetadata metadata);

    AssetMetadata Deserialize(IVirtualFile file);
}
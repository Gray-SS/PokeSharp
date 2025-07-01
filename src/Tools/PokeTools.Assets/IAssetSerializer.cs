using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets;

public interface IAssetSerializer
{
    AssetSerializerAttribute Metadata { get; }

    Result Serialize(IAsset asset, BinaryWriter writer);
}
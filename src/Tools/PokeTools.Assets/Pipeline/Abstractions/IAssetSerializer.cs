using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets.Pipeline.Abstractions;

public interface IAssetSerializer
{
    AssetSerializerAttribute Metadata { get; }

    Result<Unit> Serialize(IAsset asset, BinaryWriter writer);
}
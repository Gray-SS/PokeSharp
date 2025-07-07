using System.Reflection;
using PokeCore.Assets;
using PokeCore.Common;
using PokeCore.Common.Results;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Pipeline.Abstractions;

namespace PokeTools.Assets.Pipeline.Serializers;

public abstract class AssetSerializer<TAsset> : IAssetSerializer
    where TAsset : class, IAsset
{
    public AssetSerializerAttribute Metadata { get; }

    public AssetSerializer()
    {
        Metadata = GetType().GetCustomAttribute<AssetSerializerAttribute>() ??
            throw new InvalidOperationException($"The asset serializer '{GetType().Name}' is not annotated with '{nameof(AssetSerializerAttribute)}'");
    }

    public abstract Result<Unit> Serialize(TAsset asset, BinaryWriter writer);

    Result<Unit> IAssetSerializer.Serialize(IAsset asset, BinaryWriter writer)
    {
        return Serialize((TAsset)asset, writer);
    }
}
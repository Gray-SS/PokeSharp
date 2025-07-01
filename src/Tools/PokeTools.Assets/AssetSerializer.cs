using System.Reflection;
using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets;

public abstract class AssetSerializer<TAsset> : IAssetSerializer
    where TAsset : class, IAsset
{
    public AssetSerializerAttribute Metadata { get; }

    public AssetSerializer()
    {
        Metadata = GetType().GetCustomAttribute<AssetSerializerAttribute>() ??
            throw new InvalidOperationException($"The asset serializer '{GetType().Name}' is not annotated with '{nameof(AssetSerializerAttribute)}'");
    }

    public abstract Result Serialize(TAsset asset, BinaryWriter writer);

    Result IAssetSerializer.Serialize(IAsset asset, BinaryWriter writer)
    {
        return Serialize((TAsset)asset, writer);
    }
}
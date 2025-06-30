using System.Reflection;
using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.External.Annotations;

namespace PokeTools.Assets.External;

public abstract class AssetProcessor<TRaw, TAsset> : IAssetProcessor
    where TRaw : notnull
    where TAsset : notnull, IAsset
{
    public AssetProcessorAttribute Metadata { get; }

    public AssetProcessor()
    {
        Metadata = GetType().GetCustomAttribute<AssetProcessorAttribute>() ??
            throw new InvalidOperationException($"The asset processor '{GetType().Name}' is not annotated with '{nameof(AssetProcessorAttribute)}'");
    }

    public abstract Result<TAsset> Process(Guid assetId, TRaw rawAsset);

    Result<IAsset> IAssetProcessor.Process(Guid assetId, object rawAsset)
    {
        return Process(assetId, (TRaw)rawAsset).Map<IAsset>(x => x);
    }
}
using PokeCore.Common;

namespace PokeTools.Assets;

public abstract class AssetProcessor<TRaw, TProcessed> : IAssetProcessor
{
    public Type RawType => typeof(TRaw);
    public Type ProcessedType => typeof(TProcessed);

    public abstract Result<TProcessed> Process(TRaw rawAsset);

    Result<object> IAssetProcessor.Process(object rawAsset)
    {
        return Process((TRaw)rawAsset);
    }
}
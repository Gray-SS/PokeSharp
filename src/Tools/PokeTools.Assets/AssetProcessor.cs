using PokeCore.Common;

namespace PokeTools.Assets;

public abstract class AssetProcessor<TIn, TOut> : IAssetProcessor
{
    public Type RawType => typeof(TIn);
    public Type ProcessedType => typeof(TOut);

    public abstract AssetType AssetType { get; }

    public abstract Result<TOut, string> Process(TIn rawAsset);

    Result<object, string> IAssetProcessor.Process(object rawAsset)
    {
        return Process((TIn)rawAsset);
    }
}
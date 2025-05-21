namespace PokeSharp.Assets;

public abstract class AssetProcessor<TIn, TOut> : IAssetProcessor
{
    public Type InputType => typeof(TIn);
    public Type OutputType => typeof(TOut);

    public abstract bool CanProcess(TIn rawAsset);
    public abstract TOut Process(TIn rawAsset);

    bool IAssetProcessor.CanProcess(object rawAsset)
    {
        return CanProcess((TIn)rawAsset);
    }

    object? IAssetProcessor.Process(object rawAsset)
    {
        return Process((TIn)rawAsset);
    }
}
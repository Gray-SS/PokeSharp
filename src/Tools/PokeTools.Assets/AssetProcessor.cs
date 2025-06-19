namespace PokeTools.Assets;

public abstract class AssetProcessor<TIn, TOut> : IAssetProcessor
{
    public Type InputType => typeof(TIn);
    public Type OutputType => typeof(TOut);

    public abstract TOut Process(TIn rawAsset);

    object? IAssetProcessor.Process(object rawAsset)
    {
        return Process((TIn)rawAsset);
    }
}
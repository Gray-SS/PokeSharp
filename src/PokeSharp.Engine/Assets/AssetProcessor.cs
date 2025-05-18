namespace PokeSharp.Engine.Assets;

public interface IAssetProcessor
{
    Type InputType { get; }
    Type OutputType { get; }

    object Process(object input);
}

public abstract class AssetProcessor<TInput, TOutput> : IAssetProcessor
    where TOutput : class
{
    public Type InputType => typeof(TInput);
    public Type OutputType => typeof(TOutput);

    public abstract TOutput Process(TInput input);

    object IAssetProcessor.Process(object input)
    {
        return Process((TInput)input);
    }
}
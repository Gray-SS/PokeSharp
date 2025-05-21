namespace PokeSharp.Assets;

public interface IAssetProcessor
{
    Type InputType { get; }
    Type OutputType { get; }

    bool CanProcess(object rawAsset);

    object? Process(object rawAsset);
}
namespace PokeSharp.Assets;

public interface IAssetProcessor
{
    Type InputType { get; }
    Type OutputType { get; }

    object? Process(object rawAsset);
}
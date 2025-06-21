using PokeCore.Common;

namespace PokeTools.Assets;

public interface IAssetProcessor
{
    Type RawType { get; }
    Type ProcessedType { get; }
    AssetType AssetType { get; }

    Result<object> Process(object rawAsset);
}
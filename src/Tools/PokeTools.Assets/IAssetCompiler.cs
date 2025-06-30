using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets;

public interface IAssetCompiler
{
    AssetCompilerAttribute Metadata { get; }

    Result Compile(IAsset asset, BinaryWriter writer);
}
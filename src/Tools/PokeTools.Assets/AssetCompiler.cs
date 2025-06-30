using System.Reflection;
using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets;

public abstract class AssetCompiler<TAsset> : IAssetCompiler
    where TAsset : class, IAsset
{
    public AssetCompilerAttribute Metadata { get; }

    public AssetCompiler()
    {
        Metadata = GetType().GetCustomAttribute<AssetCompilerAttribute>() ??
            throw new InvalidOperationException($"The asset compiler '{GetType().Name}' is not annotated with '{nameof(AssetCompilerAttribute)}'");
    }

    public abstract Result Compile(TAsset asset, BinaryWriter writer);

    Result IAssetCompiler.Compile(IAsset asset, BinaryWriter writer)
    {
        return Compile((TAsset)asset, writer);
    }
}
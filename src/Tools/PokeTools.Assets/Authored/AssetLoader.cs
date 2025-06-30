using System.Reflection;
using PokeCore.Assets;
using PokeTools.Assets.Authored.Annotations;

namespace PokeTools.Assets.Authored;

public abstract class AssetLoader<TAsset, TDescriptor> : IAssetLoader
    where TAsset : IAsset
    where TDescriptor : struct
{
    public AssetLoaderAttribute Metadata { get; }
    public Type DescriptorType => typeof(TDescriptor);

    public AssetLoader()
    {
        Metadata = GetType().GetCustomAttribute<AssetLoaderAttribute>() ??
            throw new InvalidOperationException($"The asset importer '{GetType().Name}' is not annotated with '{nameof(AssetLoaderAttribute)}'");
    }

    public abstract TAsset Load(Guid assetId, TDescriptor descriptor);

    IAsset IAssetLoader.Load(Guid assetId, object descriptor)
        => Load(assetId, (TDescriptor)descriptor);
}
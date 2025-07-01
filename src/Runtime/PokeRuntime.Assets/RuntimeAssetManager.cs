using System.Diagnostics;
using PokeCore.Assets;
using PokeCore.Assets.Bundles;
using PokeCore.Common;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeRuntime.Assets;

public sealed class RuntimeAssetManager(
    IServiceResolver services
) : IAssetManager
{
    private AssetBundle _bundle = null!;
    private readonly Dictionary<Guid, IRuntimeAsset> _loadedAssets = new();

    public Result LoadBundle(string bundlePath)
    {
        if (!File.Exists(bundlePath))
            return Result.Failure(new($"No file at path '{bundlePath}' exists."));

        var stream = File.Open(bundlePath, FileMode.Open);
        var reader = new BinaryReader(stream);

        AssetBundleHeader header = AssetBundleHeader.ReadFrom(reader);

        long manifestLength = header.DataOffset - header.TableOffset;
        AssetManifest manifest = AssetManifest.ReadFrom(reader, manifestLength);
        stream.Position = header.DataOffset;

        _bundle = new AssetBundle(header, manifest, dataStream: stream);
        return Result.Success();
    }

    public IRuntimeAsset Load(Guid assetId)
    {
        if (_loadedAssets.TryGetValue(assetId, out IRuntimeAsset? asset))
            return asset;

        AssetBundleEntry? entry = _bundle.Manifest.GetEntry(assetId)
            ?? throw new InvalidOperationException($"No asset with id '{assetId}' exists");

        foreach (Guid dependency in entry.Dependencies)
            Load(dependency);

        Stream stream = _bundle.DataStream;
        stream.Seek(_bundle.DataOffset + entry.Offset, SeekOrigin.Begin);

        BinaryReader reader = new(stream);

        IRuntimeAssetLoader loader = GetLoader(entry.AssetType);
        asset = loader.Load(assetId, reader);

        _loadedAssets[entry.AssetId] = asset;
        return asset;
    }

    private IRuntimeAssetLoader GetLoader(AssetType assetType)
    {
        return services.GetServices<IRuntimeAssetLoader>()
                       .FirstOrDefault(x => x.AssetType == assetType) ??
                       throw new InvalidOperationException($"No runtime asset loader for assets of type '{assetType}'");
    }
}
using PokeCore.Assets;
using PokeCore.Assets.Bundles;
using PokeCore.Common;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeRuntime.Assets;

public sealed class RuntimeAssetManager(
    IServiceResolver service
) : IAssetManager
{
    private AssetBundle _bundle = null!;

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

    public T Load<T>(Guid assetId) where T : IRuntimeAsset
    {
        AssetBundleEntry? entry = _bundle.Manifest.Entries.FirstOrDefault(x => x.AssetId == assetId)
            ?? throw new InvalidOperationException($"No asset with id '{assetId}' exists");

        Stream stream = _bundle.DataStream;
        stream.Seek(_bundle.DataOffset + entry.Offset, SeekOrigin.Begin);

        using BinaryReader reader = new(stream);

        if (!service.TryGetService(out IRuntimeAssetLoader<T>? loader))
            throw new InvalidOperationException($"The asset loader for asset '{typeof(T).Name}' isn't registered.");

        return loader.Load(assetId, reader);
    }
}
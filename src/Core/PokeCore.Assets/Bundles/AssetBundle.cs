namespace PokeCore.Assets.Bundles;

public sealed class AssetBundle
{
    public uint Version => _header.Version;
    public uint AssetsCount => _header.AssetsCount;
    public uint TableOffset => _header.TableOffset;
    public uint DataOffset => _header.DataOffset;
    public AssetManifest Manifest { get; }
    public Stream DataStream { get; }

    private readonly AssetBundleHeader _header;

    public AssetBundle(AssetBundleHeader header, AssetManifest manifest, Stream dataStream)
    {
        Manifest = manifest;
        DataStream = dataStream;

        _header = header;
    }
}
using PokeSharp.Assets.VFS;
using PokeSharp.Core.Logging;

namespace PokeSharp.Assets.Services;

public sealed class LibraryMetadataStore : IAssetMetadataStore
{
    private readonly ILogger _logger;
    private readonly VirtualPath _libsRoot;
    private readonly IVirtualFileSystem _vfs;
    private readonly IAssetMetadataSerializer _serializer;

    public LibraryMetadataStore(ILogger logger, IVirtualFileSystem vfs, IAssetMetadataSerializer serializer)
    {
        _vfs = vfs;
        _logger = logger;
        _serializer = serializer;
        _libsRoot = VirtualPath.Parse("libs://");
    }

    public bool Exists(VirtualPath assetPath)
    {
        return _vfs.Exists(GetMetadataPath(assetPath));
    }

    public IVirtualFile GetMetadataFile(VirtualPath assetPath)
    {
        VirtualPath metadataPath = GetMetadataPath(assetPath);
        return _vfs.GetFile(metadataPath);
    }

    public VirtualPath GetMetadataPath(VirtualPath assetPath)
    {
        return _libsRoot.Combine(assetPath.Scheme + "/")
                        .Combine(assetPath.LocalPath + ".meta");
    }

    public AssetMetadata Load(VirtualPath assetPath)
    {
        IVirtualFile file = GetMetadataFile(assetPath);
        if (!file.Exists)
            throw new FileNotFoundException($"Metadata of asset '{assetPath}' wasn't found at path '{file.Path}'.");

        return _serializer.Deserialize(file);
    }

    public void Save(VirtualPath assetPath, AssetMetadata metadata)
    {
        VirtualPath metadataPath = GetMetadataPath(assetPath);
        IVirtualFile file = _vfs.CreateFile(metadataPath);
        _serializer.Serialize(file, metadata);
    }

    public void DeleteAll()
    {
        if (!_vfs.IsVolumeMounted(_libsRoot.Scheme))
            return;

        IVirtualDirectory directory = _vfs.GetDirectory(_libsRoot);

        foreach (var child in directory.GetDirectories())
            child.Delete();

        foreach (var child in directory.GetFiles())
            child.Delete();
    }
}
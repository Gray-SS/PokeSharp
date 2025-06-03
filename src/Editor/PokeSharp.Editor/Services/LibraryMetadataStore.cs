using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Services;
using PokeSharp.Assets.VFS.Volumes;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.Services;

namespace PokeSharp.Assets.Services;

public sealed class LibraryMetadataStore : IAssetMetadataStore
{
    private const string LibsScheme = "libs";

    private readonly ILogger _logger;
    private readonly VirtualPath _libsRoot;
    private readonly IVirtualFileSystem _vfs;
    private readonly IProjectManager _projectManager;
    private readonly IVirtualVolumeManager _volumeManager;
    private readonly IAssetMetadataSerializer _serializer;

    public LibraryMetadataStore(
        ILogger logger,
        IVirtualFileSystem vfs,
        IProjectManager projectManager,
        IVirtualVolumeManager volumeManager,
        IAssetMetadataSerializer serializer)
    {
        _vfs = vfs;
        _logger = logger;
        _serializer = serializer;
        _volumeManager = volumeManager;
        _projectManager = projectManager;
        _libsRoot = VirtualPath.Parse("libs://");
    }

    public bool Exists(VirtualPath assetPath)
    {
        return _vfs.FileExists(GetMetadataPath(assetPath));
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
        IVirtualFile file = _vfs.CreateFile(metadataPath, true);
        _serializer.Serialize(file, metadata);
    }

    public void DeleteAll()
    {
        if (!_projectManager.HasActiveProject)
            return;

        IVirtualVolume? volume = _projectManager.ActiveProject!.LibsVolume;
        foreach (var child in volume.RootDirectory.GetDirectories())
            child.Delete();

        foreach (var child in volume.RootDirectory.GetFiles())
            child.Delete();
    }
}
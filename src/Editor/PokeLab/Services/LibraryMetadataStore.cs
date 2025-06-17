using PokeEngine.Assets;
using PokeEngine.Assets.Services;
using PokeCore.IO;
using PokeCore.IO.Events;
using PokeCore.IO.Services;
using PokeCore.IO.Volumes;
using PokeCore.Hosting.Logging;
using PokeEngine.Core.Threadings;

namespace PokeLab.Services;

public sealed class LibraryMetadataStore : IAssetMetadataStore
{
    private int _bulkOperations;

    private readonly Logger _logger;
    private readonly IVirtualFileSystem _vfs;
    private readonly IAssetMetadataSerializer _serializer;

    private readonly Dictionary<VirtualPath, Guid> _pathToIdLookup;
    private readonly Dictionary<Guid, AssetMetadata> _idToMetadataLookup;

    public LibraryMetadataStore(
        Logger logger,
        IVirtualFileSystem vfs,
        IAssetMetadataSerializer serializer)
    {
        _logger = logger;

        _vfs = vfs;
        _vfs.OnFileSystemChanged += OnFileSystemChanged;

        _serializer = serializer;
        _pathToIdLookup = new Dictionary<VirtualPath, Guid>();
        _idToMetadataLookup = new Dictionary<Guid, AssetMetadata>();
    }

    private void OnFileSystemChanged(object? sender, FileSystemChangedArgs e)
    {
        if (!Project.IsActive || _bulkOperations > 0) return;

        if (e.VirtualPath.IsDirectory || !Project.Active.LibsVolume.RootPath.IsParentOf(e.VirtualPath))
            return;

        if (e.ChangeType == FileSystemChangeType.Created)
        {
            _logger.Trace("Asset metadata created. Loading them");
        }
        else
        {
            _logger.Trace($"Invalidating '{e.VirtualPath}'");
            if (_pathToIdLookup.Remove(e.VirtualPath, out Guid assetId))
            {
                _idToMetadataLookup.Remove(assetId);
                _logger.Trace($"Invalidated metadata cache {e.VirtualPath} ({assetId})");
            }
            else _logger.Trace($"No metadata cached for '{e.VirtualPath}'");
        }
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
        return Project.Active.LibsVolume.RootPath.Combine(assetPath.Scheme + "/")
                                                 .Combine(assetPath.LocalPath + ".meta");
    }

    public AssetMetadata Load(VirtualPath assetPath)
    {
        _logger.Trace($"Loading asset metadata for asset: '{assetPath}'");
        if (_pathToIdLookup.TryGetValue(assetPath, out Guid id))
        {
            _logger.Trace($"Metadata cache hit. Returning cached asset metadata.");
            return _idToMetadataLookup[id];
        }

        IVirtualFile metadataFile = GetMetadataFile(assetPath);
        if (!metadataFile.Exists)
            throw new FileNotFoundException($"Metadata of asset '{assetPath}' wasn't found at path '{metadataFile.Path}'.");

        AssetMetadata metadata = _serializer.Deserialize(metadataFile);

        _logger.Trace($"Asset metadata cached with key '{metadataFile.Path}' -> '{metadata.Id}'");
        _pathToIdLookup[metadataFile.Path] = metadata.Id;
        _idToMetadataLookup[metadata.Id] = metadata;
        return metadata;
    }

    public void Save(VirtualPath assetPath, AssetMetadata metadata)
    {
        VirtualPath metadataPath = GetMetadataPath(assetPath);
        _logger.Trace($"Saving asset metadata at '{metadataPath}' ({metadata.Id}).");
        IVirtualFile file = _vfs.CreateFile(metadataPath, true);
        if (!file.Exists)
        {
            _logger.Warn($"Failed creating metadata file '{metadataPath}'");
            return;
        }

        _serializer.Serialize(file, metadata);

        _pathToIdLookup[metadataPath] = metadata.Id;
        _idToMetadataLookup[metadata.Id] = metadata;
        _logger.Trace($"Asset metadata cached with key '{metadataPath}' -> '{metadata.Id}'");
        _logger.Trace($"Asset metadata saved to '{metadataPath}' ({metadata.Id})");
    }

    public AssetMetadata? GetMetadata(VirtualPath assetPath)
    {
        if (_pathToIdLookup.TryGetValue(GetMetadataPath(assetPath), out Guid assetId))
            return _idToMetadataLookup[assetId];

        return null;
    }

    public AssetMetadata? GetMetadata(Guid assetId)
    {
        _idToMetadataLookup.TryGetValue(assetId, out AssetMetadata? metadata);
        return metadata;
    }

    public IEnumerable<AssetMetadata> GetCachedMetadatas()
    {
        return _idToMetadataLookup.Values;
    }

    public void EnterBulkMode()
    {
        _bulkOperations++;
        _logger.Debug($"Bulk mode started ({_bulkOperations})");
    }

    public void ExitBulkMode()
    {
        _logger.Debug($"Dispatching bulk mode end ({_bulkOperations})");

        ThreadHelper.RunOnMainThread(() =>
        {
            _logger.Debug($"Bulk mode ended ({_bulkOperations})");
            _bulkOperations--;
        });
    }

    public void DeleteAll()
    {
        if (!Project.IsActive)
        {
            _logger.Warn("No project is active. Cannot delete the metadata files and directories.");
            return;
        }

        EnterBulkMode();
        _logger.Trace("Deleting all metadata files and directories");

        _pathToIdLookup.Clear();
        _idToMetadataLookup.Clear();

        PhysicalVolume volume = Project.Active.LibsVolume;
        foreach (var child in volume.RootDirectory.GetDirectories())
            child.Delete();

        foreach (var child in volume.RootDirectory.GetFiles())
            child.Delete();

        _logger.Trace("Deleted all metadata files and directories.");
        ExitBulkMode();
    }
}
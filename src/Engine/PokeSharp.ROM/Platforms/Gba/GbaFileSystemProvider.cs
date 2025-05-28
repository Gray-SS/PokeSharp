using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Events;
using PokeSharp.Core.Logging;

namespace PokeSharp.ROM.Platforms.Gba;

public sealed class GbaFileSystemProvider : IVirtualFileSystemProvider
{
    public event EventHandler<FileSystemChangedArgs>? OnFileChanged;

    private readonly ILogger _logger;

    public GbaFileSystemProvider(Rom rom)
    {
        _logger = LoggerFactory.GetLogger(typeof(GbaFileSystemProvider));
    }

    public IVirtualDirectory GetDirectory(VirtualPath virtualPath)
    {
        // rom://graphics/
        string[] folders = virtualPath.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        foreach (string folder in folders)
        {
            _logger.Info($"Folder path: {folder}");
        }

        return null;
    }

    public IVirtualFile GetFile(VirtualPath virtualPath)
    {
        return null;
    }

    public IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath)
    {
        return [];
    }

    public IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath)
    {
        return [];
    }

    public StreamReader OpenRead(VirtualPath virtualPath)
    {
        return null!;
    }

    public IVirtualDirectory CreateDirectory(VirtualPath virtualPath)
    {
        throw new NotSupportedException();
    }

    public IVirtualFile CreateFile(VirtualPath virtualPath, bool overwrite)
    {
        throw new NotSupportedException();
    }

    public StreamWriter OpenWrite(VirtualPath virtualPath)
    {
        throw new NotSupportedException();
    }

    public string CombinePath(params string[] paths)
    {
        throw new NotImplementedException();
    }

    public IVirtualEntry RenameEntry(VirtualPath virtualPath, string name)
    {
        throw new NotSupportedException();
    }

    public IVirtualEntry DuplicateEntry(VirtualPath virtualPath)
    {
        throw new NotSupportedException();
    }

    public bool Exists(VirtualPath virtualPath)
    {
        throw new NotImplementedException();
    }

    public bool DeleteEntry(VirtualPath virtualPath)
    {
        throw new NotImplementedException();
    }
}
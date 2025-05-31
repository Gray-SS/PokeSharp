using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Events;
using PokeSharp.Core.Logging;
using PokeSharp.ROM.Services;

namespace PokeSharp.ROM;

public sealed class RomFileSystemProvider : IVirtualFileSystemProvider
{
    public event EventHandler<FileSystemChangedArgs>? OnFileChanged;

    private readonly ILogger _logger;

    private readonly Dictionary<VirtualPath, RomFile> _files;
    private readonly Dictionary<VirtualPath, RomDirectory> _directories;

    public RomFileSystemProvider(VirtualPath rootPath, RomVfsBuilder vfsBuilder)
    {
        ArgumentNullException.ThrowIfNull(rootPath);
        ArgumentNullException.ThrowIfNull(vfsBuilder);

        _logger = LoggerFactory.GetLogger(typeof(RomFileSystemProvider));
        _files = new Dictionary<VirtualPath, RomFile>();
        _directories = new Dictionary<VirtualPath, RomDirectory>();

        BuildVirtualFileSystem(vfsBuilder, rootPath);
    }

    private void BuildVirtualFileSystem(RomVfsBuilder builder, VirtualPath rootPath)
    {
        var root = new RomDirectory(rootPath);
        builder.Build(root);

        RegisterDirectoryRecursive(root);
    }

    private void RegisterDirectoryRecursive(RomDirectory directory)
    {
        _directories.Add(directory.Path, directory);
        foreach (RomEntry entry in directory)
        {
            if (entry is RomDirectory dir)
            {
                RegisterDirectoryRecursive(dir);
            }
            else if (entry is RomFile file)
            {
                _files.Add(file.Path, file);
            }
            else throw new NotSupportedException($"Failed registered directories - Entries of type '{entry.GetType().Name}' are not supported");
        }
    }

    public IVirtualDirectory GetDirectory(VirtualPath virtualPath)
    {
        if (!virtualPath.IsDirectory)
            throw new ArgumentException($"Getting directory failed - the virtual path doesn't lead to a directory: {virtualPath}");

        return new VirtualDirectory(this, virtualPath);
    }

    public IVirtualFile GetFile(VirtualPath virtualPath)
    {
        if (virtualPath.IsDirectory)
            throw new ArgumentException($"Getting file failed - the virtual path doesn't lead to a file: {virtualPath}");

        return new VirtualFile(this, virtualPath);
    }

    public IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath)
    {
        if (!_directories.TryGetValue(virtualPath, out RomDirectory? directory))
            yield break;

        foreach (RomFile entry in directory.GetFiles())
        {
            yield return GetFile(entry.Path);
        }
    }

    public IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath)
    {
        if (!_directories.TryGetValue(virtualPath, out RomDirectory? directory))
            yield break;

        foreach (RomDirectory entry in directory.GetDirectories())
        {
            yield return GetDirectory(entry.Path);
        }
    }

    public Stream OpenRead(VirtualPath virtualPath)
    {
        var bytes = ReadBytes(virtualPath);
        var stream = new MemoryStream(bytes, false);
        return stream;
    }

    public byte[] ReadBytes(VirtualPath virtualPath)
    {
        if (!_files.TryGetValue(virtualPath, out RomFile? file))
            throw new FileNotFoundException($"No file at path '{virtualPath}' was found");

        return file.Data.ToArray();
    }

    public IVirtualDirectory CreateDirectory(VirtualPath virtualPath)
    {
        throw new NotSupportedException();
    }

    public IVirtualFile CreateFile(VirtualPath virtualPath, bool overwrite)
    {
        throw new NotSupportedException();
    }

    public Stream OpenWrite(VirtualPath virtualPath)
    {
        throw new NotSupportedException();
    }

    public IVirtualEntry MoveEntry(VirtualPath srcPath, VirtualPath destPath)
    {
        throw new NotSupportedException();
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
        return FileExists(virtualPath) || DirectoryExists(virtualPath);
    }

    public bool FileExists(VirtualPath virtualPath)
    {
        return _files.ContainsKey(virtualPath);
    }

    public bool DirectoryExists(VirtualPath virtualPath)
    {
        return _directories.ContainsKey(virtualPath);
    }

    public bool DeleteEntry(VirtualPath virtualPath)
    {
        throw new NotImplementedException();
    }
}
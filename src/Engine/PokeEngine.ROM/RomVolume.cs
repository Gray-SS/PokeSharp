using PokeEngine.Assets.VFS;
using PokeEngine.Assets.VFS.Events;
using PokeEngine.Assets.VFS.Volumes;
using PokeCore.Hosting.Logging;
using PokeEngine.ROM.Services;

namespace PokeEngine.ROM;

public sealed class RomVolume : BaseVirtualVolume, IReadableVolume
{
    private readonly Logger _logger;

    private readonly Dictionary<VirtualPath, RomFile> _files;
    private readonly Dictionary<VirtualPath, RomDirectory> _directories;

    public RomVolume(string id, string scheme, string displayName, RomVfsBuilder vfsBuilder) : base(id, scheme, displayName)
    {
        ArgumentNullException.ThrowIfNull(vfsBuilder);

        _logger = LoggerFactory.GetLogger(typeof(RomVolume));
        _files = new Dictionary<VirtualPath, RomFile>();
        _directories = new Dictionary<VirtualPath, RomDirectory>();

        BuildVirtualFileSystem(vfsBuilder);
    }

    private void BuildVirtualFileSystem(RomVfsBuilder builder)
    {
        var root = new RomDirectory(RootPath);
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

    public override IVirtualDirectory GetDirectory(VirtualPath virtualPath)
    {
        if (!virtualPath.IsDirectory)
            throw new ArgumentException($"Getting directory failed - the virtual path doesn't lead to a directory: {virtualPath}");

        return new VirtualDirectory(this, virtualPath);
    }

    public override IVirtualFile GetFile(VirtualPath virtualPath)
    {
        if (virtualPath.IsDirectory)
            throw new ArgumentException($"Getting file failed - the virtual path doesn't lead to a file: {virtualPath}");

        return new VirtualFile(this, virtualPath);
    }

    public override IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath)
    {
        if (!_directories.TryGetValue(virtualPath, out RomDirectory? directory))
            yield break;

        foreach (RomFile entry in directory.GetFiles())
        {
            yield return GetFile(entry.Path);
        }
    }

    public override IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath)
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

    public override bool EntryExists(VirtualPath virtualPath)
    {
        return FileExists(virtualPath) || DirectoryExists(virtualPath);
    }

    public override bool FileExists(VirtualPath virtualPath)
    {
        return _files.ContainsKey(virtualPath);
    }

    public override bool DirectoryExists(VirtualPath virtualPath)
    {
        return _directories.ContainsKey(virtualPath);
    }
}
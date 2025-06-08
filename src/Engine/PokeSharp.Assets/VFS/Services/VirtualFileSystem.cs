using PokeSharp.Assets.VFS.Events;
using PokeSharp.Assets.VFS.Extensions;
using PokeSharp.Assets.VFS.Volumes;
using PokeSharp.Core;
using PokeSharp.Core.Logging;

namespace PokeSharp.Assets.VFS.Services;

public sealed class VirtualFileSystem : IVirtualFileSystem, IDisposable
{
    public event EventHandler<VirtualVolumeEventArgs>? OnVolumeMounted;
    public event EventHandler<VirtualVolumeEventArgs>? OnVolumeUnmounted;
    public event EventHandler<FileSystemChangedArgs>? OnFileSystemChanged;

    private bool _disposed;
    private readonly Logger _logger;
    private readonly IVirtualVolumeManager _volumeManager;

    public VirtualFileSystem(IVirtualVolumeManager volumeManager, Logger logger)
    {
        _logger = logger;

        _volumeManager = volumeManager;
        _volumeManager.OnVolumeMounted += HandleOnVolumeMounted;
        _volumeManager.OnVolumeUnmounted += HandleOnVolumeUnmounted;
        _volumeManager.OnFileSystemChanged += HandleOnFileSystemChanged;
    }

    private void HandleOnVolumeMounted(object? sender, VirtualVolumeEventArgs e)
    {
        OnVolumeMounted?.Invoke(sender, e);
    }

    private void HandleOnVolumeUnmounted(object? sender, VirtualVolumeEventArgs e)
    {
        OnVolumeUnmounted?.Invoke(sender, e);
    }

    private void HandleOnFileSystemChanged(object? sender, FileSystemChangedArgs e)
    {
        _logger.Trace($"Watchable volume triggered file system changed. ({(e.VirtualPath.IsDirectory ? "Directory" : "File")} {e.ChangeType.ToString().ToLower()} '{e.VirtualPath}')");
        OnFileSystemChanged?.Invoke(sender, e);
    }

    public bool EntryExists(VirtualPath virtualPath)
    {
        IFetchableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsFetchable();
        return volume.EntryExists(virtualPath);
    }

    public bool FileExists(VirtualPath virtualPath)
    {
        IFetchableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsFetchable();
        return volume.FileExists(virtualPath);
    }

    public bool DirectoryExists(VirtualPath virtualPath)
    {
        IFetchableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsFetchable();
        return volume.DirectoryExists(virtualPath);
    }

    public IVirtualEntry GetEntry(VirtualPath virtualPath)
    {
        IFetchableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsFetchable();
        return volume.GetEntry(virtualPath);
    }

    public IVirtualFile GetFile(VirtualPath virtualPath)
    {
        IFetchableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsFetchable();
        return volume.GetFile(virtualPath);
    }

    public IVirtualDirectory GetDirectory(VirtualPath virtualPath)
    {
        IFetchableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsFetchable();
        return volume.GetDirectory(virtualPath);
    }

    public IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath)
    {
        IFetchableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsFetchable();
        return volume.GetFiles(virtualPath);
    }

    public IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath)
    {
        IFetchableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsFetchable();
        return volume.GetDirectories(virtualPath);
    }

    public bool DeleteEntry(VirtualPath vpath)
    {
        IWritableVolume volume = _volumeManager.GetVolumeForPath(vpath).AsWriteable();
        _logger.Trace($"Deleting virtual {(vpath.IsFile ? "file" : "directory")} at '{vpath}'");
        return volume.DeleteEntry(vpath);
    }

    public IVirtualEntry RenameEntry(VirtualPath vpath, string newName)
    {
        IWritableVolume volume = _volumeManager.GetVolumeForPath(vpath).AsWriteable();
        _logger.Trace($"Renaming virtual {(vpath.IsFile ? "file" : "directory")} from '{vpath.Name}' to '{newName}' at '{vpath}'");
        return volume.RenameEntry(vpath, newName);
    }

    public IVirtualEntry DuplicateEntry(VirtualPath vpath)
    {
        IWritableVolume volume = _volumeManager.GetVolumeForPath(vpath).AsWriteable();
        _logger.Trace($"Duplicating virtual {(vpath.IsFile ? "file" : "directory")} at '{vpath}'");
        return volume.DuplicateEntry(vpath);
    }

    public IVirtualEntry MoveEntry(VirtualPath vpath, VirtualPath newPath)
    {
        IWritableVolume volume = _volumeManager.GetVolumeForPath(vpath).AsWriteable();
        _logger.Trace($"Moving virtual {(vpath.IsFile ? "file" : "directory")} from '{vpath}' to '{newPath}'");
        return volume.MoveEntry(vpath, newPath);
    }

    public IVirtualFile CreateFile(VirtualPath virtualPath, bool overwrite = false)
    {
        IWritableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsWriteable();

        ThrowHelper.Assert(virtualPath.IsFile, "Cannot create a file at a path representing a directory.");
        _logger.Trace($"Creating file at '{virtualPath}'");
        return volume.CreateFile(virtualPath, overwrite);
    }

    public IVirtualDirectory CreateDirectory(VirtualPath virtualPath)
    {
        IWritableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsWriteable();

        ThrowHelper.Assert(virtualPath.IsFile, "Cannot create a directory at a path representing a file.");
        _logger.Trace($"Creating directory at '{virtualPath}'");
        return volume.CreateDirectory(virtualPath);
    }

    public Stream OpenWrite(VirtualPath virtualPath)
    {
        IWritableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsWriteable();
        return volume.OpenWrite(virtualPath);
    }

    public StreamWriter OpenWriter(VirtualPath virtualPath)
    {
        IWritableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsWriteable();
        return volume.OpenWriter(virtualPath);
    }

    public Stream OpenRead(VirtualPath virtualPath)
    {
        IReadableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsReadable();
        return volume.OpenRead(virtualPath);
    }

    public byte[] ReadBytes(VirtualPath virtualPath)
    {
        IReadableVolume volume = _volumeManager.GetVolumeForPath(virtualPath).AsReadable();
        return volume.ReadBytes(virtualPath);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _volumeManager.OnFileSystemChanged -= OnFileSystemChanged;
                _volumeManager.OnVolumeMounted -= OnVolumeMounted;
                _volumeManager.OnVolumeUnmounted -= OnVolumeUnmounted;
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

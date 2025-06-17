using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using PokeCore.Logging;
using PokeCore.IO.Events;
using PokeCore.IO.Volumes;

namespace PokeCore.IO.Services;

public sealed class VirtualVolumeManager : IVirtualVolumeManager
{
    public event EventHandler<VirtualVolumeEventArgs>? OnVolumeMounted;
    public event EventHandler<VirtualVolumeEventArgs>? OnVolumeUnmounted;
    public event EventHandler<FileSystemChangedArgs>? OnFileSystemChanged;

    private readonly Logger _logger;
    private readonly Dictionary<string, IVirtualVolume> _mountedVolumes;
    private bool disposedValue;

    public VirtualVolumeManager(Logger<VirtualVolumeManager> logger)
    {
        _logger = logger;
        _mountedVolumes = new Dictionary<string, IVirtualVolume>();
    }

    public void MountVolume(IVirtualVolume volume)
    {
        ValidateVolume(volume);

        if (TryGetVolumeForScheme(volume.Scheme, out IVirtualVolume? conflictedVolume))
        {
            _logger.Warn($"Tried to mount volume '{volume.DisplayName}' at scheme '{volume.Scheme}' but '{conflictedVolume.DisplayName}' is already mounted at that scheme.");
            return;
        }

        if (volume is IWatchableVolume watchable)
            watchable.OnFileSystemChanged += HandleOnFileSystemChanged;

        _mountedVolumes[volume.Scheme] = volume;
        OnVolumeMounted?.Invoke(this, new VirtualVolumeEventArgs(volume, this));
    }

    private void HandleOnFileSystemChanged(object? sender, FileSystemChangedArgs e)
    {
        OnFileSystemChanged?.Invoke(sender, e);
    }

    public void UnmountAll()
    {
        foreach (IVirtualVolume volume in _mountedVolumes.Values)
        {
            UnmountVolume(volume);
        }
    }

    public void UnmountVolume(IVirtualVolume volume)
    {
        ValidateVolume(volume);

        if (!IsVolumeMountedForScheme(volume.Scheme))
        {
            _logger.Warn($"Tried to unmount volume '{volume.DisplayName}' at scheme '{volume.Scheme}' but volume was not mounted");
            return;
        }

        if (_mountedVolumes.Remove(volume.Scheme))
        {
            OnVolumeUnmounted?.Invoke(this, new VirtualVolumeEventArgs(volume, this));
        }
    }

    public void UnmountVolume(string scheme)
    {
        ValidateScheme(scheme);

        if (!IsVolumeMountedForScheme(scheme))
        {
            _logger.Warn($"Tried to unmount volume at specified scheme '{scheme}' but no volume was mounted at this scheme.");
            return;
        }

        if (_mountedVolumes.Remove(scheme, out IVirtualVolume? volume))
        {
            OnVolumeUnmounted?.Invoke(this, new VirtualVolumeEventArgs(volume, this));
        }
    }

    public IVirtualVolume GetVolumeForPath(VirtualPath path)
    {
        ValidatePath(path);
        return GetVolumeForScheme(path.Scheme);
    }

    public IVirtualVolume GetVolumeForScheme(string scheme)
    {
        ValidateScheme(scheme);
        if (!_mountedVolumes.TryGetValue(scheme, out IVirtualVolume? volume))
        {
            string error = $"No volume is mounted with the specified scheme '{scheme}'";
            _logger.Fatal(error);
            throw new KeyNotFoundException(error);
        }

        return volume;
    }

    public bool IsVolumeMountedForScheme(string scheme)
    {
        ValidateScheme(scheme);
        return _mountedVolumes.ContainsKey(scheme);
    }

    public bool IsVolumeMountedForPath(VirtualPath path)
    {
        ValidatePath(path);
        return _mountedVolumes.ContainsKey(path.Scheme);
    }

    public bool TryGetVolumeForScheme(string scheme, [NotNullWhen(true)] out IVirtualVolume? volume)
    {
        ValidateScheme(scheme);
        return _mountedVolumes.TryGetValue(scheme, out volume);
    }

    public bool TryGetVolumeForPath(VirtualPath path, [NotNullWhen(true)] out IVirtualVolume? volume)
    {
        ValidatePath(path);
        return _mountedVolumes.TryGetValue(path.Scheme, out volume);
    }

    public IReadOnlyCollection<IVirtualVolume> GetVolumes()
    {
        return _mountedVolumes.Values;
    }

    private static void ValidatePath(VirtualPath path)
    {
        Debug.Assert(path != null, "Specified path must be not null");
        ArgumentNullException.ThrowIfNull(path);
    }

    private static void ValidateScheme(string scheme)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(scheme), "Specified scheme must be not null nor empty");
        ArgumentException.ThrowIfNullOrWhiteSpace(scheme);
    }

    private static void ValidateVolume(IVirtualVolume volume)
    {
        Debug.Assert(volume != null, "Specified volume must be not null");
        ArgumentNullException.ThrowIfNull(volume);
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (IVirtualVolume volume in _mountedVolumes.Values)
                {
                    UnmountVolume(volume);

                    if (volume is IWatchableVolume watchable)
                        watchable.OnFileSystemChanged -= HandleOnFileSystemChanged;

                    if (volume is IDisposable disposable)
                        disposable.Dispose();
                }

                _mountedVolumes.Clear();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
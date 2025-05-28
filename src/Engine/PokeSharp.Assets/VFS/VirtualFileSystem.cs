using PokeSharp.Assets.VFS.Events;
using PokeSharp.Core.Logging;

namespace PokeSharp.Assets.VFS;

public sealed class VirtualFileSystem : IVirtualFileSystem
{
    private ILogger _logger;
    private readonly Dictionary<string, VolumeInfo> _mountedVolumes;
    private readonly Dictionary<string, IVirtualFileSystemProvider> _providers;

    public event EventHandler<VolumeInfo>? OnVolumeMounted;
    public event EventHandler<VolumeInfo>? OnVolumeUnmounted;
    public event EventHandler<FileSystemChangedArgs>? OnFileChanged;

    public VirtualFileSystem(ILogger logger)
    {
        _logger = logger;
        _mountedVolumes = new Dictionary<string, VolumeInfo>();
        _providers = new Dictionary<string, IVirtualFileSystemProvider>();
    }

    public bool Exists(VirtualPath virtualPath)
    {
        IVirtualFileSystemProvider provider = GetProviderFromVirtualPath(virtualPath);
        return provider.Exists(virtualPath);
    }

    public IVirtualFile CreateFile(VirtualPath virtualPath, bool overwrite = false)
    {
        VolumeInfo volume = GetVolume(virtualPath.Scheme);
        EnsureAccess(volume, FileSystemAccess.Write);

        IVirtualFileSystemProvider provider = GetProviderFromVirtualPath(virtualPath);
        return provider.CreateFile(virtualPath, overwrite);
    }

    public IVirtualDirectory CreateDirectory(VirtualPath virtualPath)
    {
        VolumeInfo volume = GetVolume(virtualPath.Scheme);
        EnsureAccess(volume, FileSystemAccess.Write);

        IVirtualFileSystemProvider provider = GetProviderFromVirtualPath(virtualPath);
        return provider.CreateDirectory(virtualPath);
    }

    public StreamWriter OpenWrite(VirtualPath virtualPath)
    {
        VolumeInfo volume = GetVolume(virtualPath.Scheme);
        EnsureAccess(volume, FileSystemAccess.Write);

        IVirtualFileSystemProvider provider = GetProviderFromVirtualPath(virtualPath);
        return provider.OpenWrite(virtualPath);
    }

    public StreamReader OpenRead(VirtualPath virtualPath)
    {
        VolumeInfo volume = GetVolume(virtualPath.Scheme);
        EnsureAccess(volume, FileSystemAccess.Read);

        IVirtualFileSystemProvider provider = GetProviderFromVirtualPath(virtualPath);
        return provider.OpenRead(virtualPath);
    }

    public IVirtualFile GetFile(VirtualPath virtualPath)
    {
        IVirtualFileSystemProvider provider = GetProviderFromVirtualPath(virtualPath);
        return provider.GetFile(virtualPath);
    }

    public IVirtualDirectory GetDirectory(VirtualPath virtualPath)
    {
        IVirtualFileSystemProvider provider = GetProviderFromVirtualPath(virtualPath);
        return provider.GetDirectory(virtualPath);
    }

    public IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath)
    {
        IVirtualFileSystemProvider provider = GetProviderFromVirtualPath(virtualPath);
        return provider.GetFiles(virtualPath);
    }

    public IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath)
    {
        IVirtualFileSystemProvider provider = GetProviderFromVirtualPath(virtualPath);
        return provider.GetDirectories(virtualPath);
    }

    public VolumeInfo GetVolume(string scheme)
    {
        _logger.Debug($"Getting volume mounted to scheme '{scheme}'.");
        if (_mountedVolumes.TryGetValue(scheme, out VolumeInfo? volume))
            return volume;

        _logger.Error($"No volume found mounted to scheme '{scheme}'");
        return null!;
    }

    public IEnumerable<VolumeInfo> GetVolumes()
    {
        return _mountedVolumes.Values;
    }

    public void MountVolume(VolumeInfo volume, IVirtualFileSystemProvider provider)
    {
        ArgumentNullException.ThrowIfNull(volume);
        ArgumentNullException.ThrowIfNull(provider);

        _logger.Trace($"Mounting volume '{volume.DisplayName}' to scheme '{volume.Scheme}' using provider '{provider.GetType().Name}'");
        if (_mountedVolumes.ContainsKey(volume.Scheme))
            throw new InvalidOperationException($"A volume with scheme '{volume.Scheme}' is already mounted.");

        provider.OnFileChanged += HandleOnFileChanged;
        _logger.Debug($"{nameof(OnFileChanged)} event has been registered.");

        _providers[volume.Scheme] = provider;
        _mountedVolumes[volume.Scheme] = volume;

        _logger.Info($"Mounted volume '{volume.DisplayName}' to scheme '{volume.Scheme}' successfully.");
        OnVolumeMounted?.Invoke(this, volume);
    }

    public void UnmountVolume(VolumeInfo volume)
    {
        _logger.Trace($"Unmounting volume '{volume.DisplayName}' from scheme '{volume.Scheme}'");
        if (_mountedVolumes.Remove(volume.Scheme, out VolumeInfo? removedVolume))
        {
            _logger.Debug($"{nameof(OnFileChanged)} event has been unregistered.");
            var attachedProvider = GetProviderFromVirtualPath(volume.RootPath);
            attachedProvider.OnFileChanged -= HandleOnFileChanged;

            OnVolumeUnmounted?.Invoke(this, removedVolume);
        }
        else _logger.Warn($"No mounted volume has been found at scheme '{volume.Scheme}'");
    }

    public void UnmountVolume(string scheme)
    {
        _logger.Trace($"Unmounting volume from scheme '{scheme}'");
        if (_mountedVolumes.Remove(scheme, out VolumeInfo? volume))
        {
            _logger.Debug($"{nameof(OnFileChanged)} event has been unregistered.");
            var attachedProvider = GetProviderFromVirtualPath(volume.RootPath);
            attachedProvider.OnFileChanged -= HandleOnFileChanged;

            OnVolumeUnmounted?.Invoke(this, volume);
        }
        else _logger.Warn($"No mounted provider has been found at mount point: '{scheme}'");
    }

    public void UnmountVolumes()
    {
        _logger.Trace($"Unmounting all the mounted volumes");
        _providers.Clear();
        _mountedVolumes.Clear();
    }

    private void HandleOnFileChanged(object? sender, FileSystemChangedArgs e)
    {
        OnFileChanged?.Invoke(sender, e);
    }

    private static void EnsureAccess(VolumeInfo volume, FileSystemAccess access)
    {
        if (!volume.HasAccessTo(access))
            throw new InvalidOperationException($"Cannot perform action to volume '{volume.DisplayName}' mounted to '{volume.Scheme}'. The required access is '{access}'");
    }

    private IVirtualFileSystemProvider GetProviderFromVirtualPath(VirtualPath virtualPath)
    {
        if (!_providers.TryGetValue(virtualPath.Scheme, out IVirtualFileSystemProvider? provider))
            throw new InvalidOperationException($"No providers mounted for '{virtualPath.Scheme}'");

        return provider;
    }
}

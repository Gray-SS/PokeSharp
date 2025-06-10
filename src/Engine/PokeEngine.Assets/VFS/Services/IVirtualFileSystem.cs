using PokeEngine.Assets.VFS.Events;
using PokeEngine.Assets.VFS.Volumes;

namespace PokeEngine.Assets.VFS.Services;

public interface IVirtualFileSystem : IFetchableVolume, IReadableVolume, IWritableVolume, IWatchableVolume
{
    /// <summary>
    /// Event triggered when a volume is successfully mounted.
    /// </summary>
    event EventHandler<VirtualVolumeEventArgs>? OnVolumeMounted;

    /// <summary>
    /// Event triggered when a volume is successfully unmounted.
    /// </summary>
    event EventHandler<VirtualVolumeEventArgs>? OnVolumeUnmounted;
}
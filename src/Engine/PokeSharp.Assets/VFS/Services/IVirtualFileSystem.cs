using PokeSharp.Assets.VFS.Events;
using PokeSharp.Assets.VFS.Volumes;

namespace PokeSharp.Assets.VFS.Services;

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
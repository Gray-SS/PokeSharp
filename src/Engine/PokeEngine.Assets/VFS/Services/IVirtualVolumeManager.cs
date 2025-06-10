using System.Diagnostics.CodeAnalysis;
using PokeEngine.Assets.VFS.Events;
using PokeEngine.Assets.VFS.Volumes;

namespace PokeEngine.Assets.VFS.Services;

/// <summary>
/// Provides volume-related functionnality, including mounting, unmounting, ect.
/// </summary>
public interface IVirtualVolumeManager : IDisposable
{
    /// <summary>
    /// Event triggered when a volume is successfully mounted.
    /// </summary>
    event EventHandler<VirtualVolumeEventArgs>? OnVolumeMounted;

    /// <summary>
    /// Event triggered when a volume is successfully unmounted.
    /// </summary>
    event EventHandler<VirtualVolumeEventArgs>? OnVolumeUnmounted;

    /// <summary>
    /// Event triggered when a volume implementing <see cref="IWatchableVolume"/> reports a file-system change.
    /// </summary>
    event EventHandler<FileSystemChangedArgs>? OnFileSystemChanged;

    /// <summary>
    /// Mounts a volume to the virtual volume manager.
    /// </summary>
    /// <param name="volume">The volume to be mounted.</param>
    /// <remarks>
    /// Fires <see cref="OnVolumeMounted"/> upon successful mount.
    /// </remarks>
    void MountVolume(IVirtualVolume volume);

    /// <summary>
    /// Unmounts the specified volume from the virtual volume manager.
    /// </summary>
    /// <param name="volume">The volume to be unmounted</param>
    /// <remarks>
    /// Fires <see cref="OnVolumeUnmounted"/> upon successful unmount.
    /// </remarks>
    void UnmountVolume(IVirtualVolume volume);

    /// <summary>
    /// Unmounts the volume at specified scheme from the virtual volume manager.
    /// </summary>
    /// <param name="volume">The volume to be unmounted</param>
    /// <remarks>
    /// Fires <see cref="OnVolumeUnmounted"/> upon successful unmount.
    /// </remarks>
    void UnmountVolume(string scheme);

    /// <summary>
    /// Unmounts all the volumes mounted from the virtual volume manager.
    /// </summary>
    /// <remarks>
    /// Fires <see cref="OnVolumeUnmounted"/> for each volume unmounted.
    /// </remarks>
    void UnmountAll();

    /// <summary>
    /// Tries to retrieve the mounted volume by scheme.
    /// </summary>
    /// <param name="scheme">The scheme identifying the volume.</param>
    /// <param name="volume">If found, the <see cref="IVirtualVolume"/>; otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if a volume with the specified scheme is mounted; otherwise, <c>false</c>.</returns>
    bool TryGetVolumeForScheme(string scheme, [NotNullWhen(true)] out IVirtualVolume? volume);

    /// <summary>
    /// Tries to retrieve the mounted volume that handles the given virtual path.
    /// </summary>
    /// <param name="path">A virtual path within a mounted volume.</param>
    /// <param name="volume">If found, the <see cref="IVirtualVolume"/>; otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if a volume for the specified path is found; otherwise, <c>false</c>.</returns>
    bool TryGetVolumeForPath(VirtualPath path, [NotNullWhen(true)] out IVirtualVolume? volume);

    /// <summary>
    /// Gets the mounted volume for the specified scheme.
    /// </summary>
    /// <param name="scheme">The scheme of the mounted volume.</param>
    /// <returns>The <see cref="IVirtualVolume"/> associated with that scheme.</returns>
    IVirtualVolume GetVolumeForScheme(string scheme);

    /// <summary>
    /// Gets the mounted volume that handles the specified virtual path.
    /// </summary>
    /// <param name="path">A virtual path within a mounted volume.</param>
    /// <returns>The <see cref="IVirtualVolume"/> handling that path.</returns>
    IVirtualVolume GetVolumeForPath(VirtualPath path);

    /// <summary>
    /// Determines if a volume is mounted for the given scheme.
    /// </summary>
    /// <param name="scheme">The scheme to check.</param>
    /// <returns><c>true</c> if a volume with that scheme is currently mounted; otherwise, <c>false</c>.</returns>
    bool IsVolumeMountedForScheme(string scheme);

    /// <summary>
    /// Determines if a volume is mounted that handles the given virtual path.
    /// </summary>
    /// <param name="path">The virtual path to check.</param>
    /// <returns><c>true</c> if a volume handling that path is currently mounted; otherwise, <c>false</c>.</returns>
    bool IsVolumeMountedForPath(VirtualPath path);

    /// <summary>
    /// Returns all currently mounted volumes.
    /// </summary>
    /// <returns>A read-only collection of all mounted <see cref="IVirtualVolume"/> instances.</returns>
    IReadOnlyCollection<IVirtualVolume> GetVolumes();
}
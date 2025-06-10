using PokeEngine.Assets.VFS.Services;
using PokeEngine.Assets.VFS.Volumes;

namespace PokeEngine.Assets.VFS.Events;

public sealed class VirtualVolumeEventArgs : EventArgs
{
    public IVirtualVolume Volume { get; }
    public IVirtualVolumeManager VolumeManager { get; }

    public VirtualVolumeEventArgs(IVirtualVolume volume, IVirtualVolumeManager volumeManager)
    {
        Volume = volume;
        VolumeManager = volumeManager;
    }
}
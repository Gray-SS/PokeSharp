using PokeSharp.Assets.VFS.Services;
using PokeSharp.Assets.VFS.Volumes;

namespace PokeSharp.Assets.VFS.Events;

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
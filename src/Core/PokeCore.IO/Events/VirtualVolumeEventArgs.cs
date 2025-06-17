using PokeCore.IO.Services;
using PokeCore.IO.Volumes;

namespace PokeCore.IO.Events;

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
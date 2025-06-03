using PokeSharp.Assets.VFS.Events;

namespace PokeSharp.Assets.VFS.Volumes;

public interface IWatchableVolume
{
    event EventHandler<FileSystemChangedArgs>? OnFileSystemChanged;
}
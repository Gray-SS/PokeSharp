using PokeEngine.Assets.VFS.Events;

namespace PokeEngine.Assets.VFS.Volumes;

public interface IWatchableVolume
{
    event EventHandler<FileSystemChangedArgs>? OnFileSystemChanged;
}
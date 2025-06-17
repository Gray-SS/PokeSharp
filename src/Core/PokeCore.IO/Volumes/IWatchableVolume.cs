using PokeCore.IO.Events;

namespace PokeCore.IO.Volumes;

public interface IWatchableVolume
{
    event EventHandler<FileSystemChangedArgs>? OnFileSystemChanged;
}
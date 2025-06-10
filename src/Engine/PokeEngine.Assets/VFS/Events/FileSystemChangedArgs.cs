namespace PokeEngine.Assets.VFS.Events;

public enum FileSystemChangeType
{
    Created,
    Renamed,
    Modified,
    Deleted,
}

public sealed class FileSystemChangedArgs : EventArgs
{
    public FileSystemChangeType ChangeType { get; }
    public VirtualPath VirtualPath { get; }

    public FileSystemChangedArgs(FileSystemChangeType type, VirtualPath virtualPath)
    {
        ChangeType = type;
        VirtualPath = virtualPath;
    }
}
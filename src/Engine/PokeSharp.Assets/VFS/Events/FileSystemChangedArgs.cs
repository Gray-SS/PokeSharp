namespace PokeSharp.Assets.VFS.Events;

public sealed class FileSystemChangedArgs : EventArgs
{
    public enum ChangeType
    {
        Created,
        Renamed,
        Modified,
        Deleted,
    }

    public ChangeType Type { get; }
    public VirtualPath VirtualPath { get; }

    public FileSystemChangedArgs(ChangeType type, VirtualPath virtualPath)
    {
        Type = type;
        VirtualPath = virtualPath;
    }
}
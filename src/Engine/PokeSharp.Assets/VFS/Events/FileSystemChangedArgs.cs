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

    public string FullVirtualPath { get; }

    public FileSystemChangedArgs(ChangeType type, string virtualPath)
    {
        Type = type;
        FullVirtualPath = virtualPath;
    }
}
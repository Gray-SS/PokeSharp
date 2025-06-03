namespace PokeSharp.Assets.VFS.Volumes;

public interface IWritableVolume
{
    bool DeleteEntry(VirtualPath virtualPath);
    IVirtualEntry RenameEntry(VirtualPath virtualPath, string name);
    IVirtualEntry DuplicateEntry(VirtualPath virtualPath);
    IVirtualEntry MoveEntry(VirtualPath srcPath, VirtualPath destPath);

    Stream OpenWrite(VirtualPath virtualPath);
    StreamWriter OpenWriter(VirtualPath virtualPath);

    IVirtualFile CreateFile(VirtualPath virtualPath, bool overwrite);
    IVirtualDirectory CreateDirectory(VirtualPath virtualPath);
}
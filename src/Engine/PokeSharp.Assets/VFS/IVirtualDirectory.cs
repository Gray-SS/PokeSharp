namespace PokeSharp.Assets.VFS;

public interface IVirtualDirectory : IVirtualEntry, IEquatable<IVirtualEntry>
{
    IVirtualFile CreateFile(string fileName, bool overwrite = false);
    IVirtualDirectory CreateDirectory(string dirName);

    bool FileExists(string fileName);
    bool DirectoryExists(string dirName);

    IVirtualFile GetFile(string fileName);
    IVirtualDirectory GetDirectory(string dirName);

    IEnumerable<IVirtualFile> GetFiles();
    IEnumerable<IVirtualDirectory> GetDirectories();
}
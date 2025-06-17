namespace PokeCore.IO;

public interface IVirtualDirectory : IVirtualEntry, IEquatable<IVirtualEntry>
{
    bool IsRoot { get; }

    IVirtualFile CreateFile(string fileName, bool overwrite = false);
    IVirtualDirectory CreateDirectory(string dirName);

    bool EntryExists(string entryName);
    bool FileExists(string fileName);
    bool DirectoryExists(string dirName);

    IVirtualFile GetFile(string fileName);
    IVirtualDirectory GetDirectory(string dirName);

    IEnumerable<IVirtualFile> GetFiles();
    IEnumerable<IVirtualDirectory> GetDirectories();
}
namespace PokeSharp.Assets.VFS;

public interface IVirtualDirectory : IVirtualEntry, IEnumerable<IVirtualEntry>
{
    IVirtualFile CreateFile(string fileName, bool overwrite = false);
    IVirtualDirectory CreateDirectory(string dirName);

    IEnumerable<IVirtualEntry> GetEntries();
}
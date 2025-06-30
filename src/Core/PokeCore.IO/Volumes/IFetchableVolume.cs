namespace PokeCore.IO.Volumes;

public interface IFetchableVolume
{
    bool EntryExists(VirtualPath virtualPath);
    bool FileExists(VirtualPath virtualPath);
    bool DirectoryExists(VirtualPath virtualPath);

    IVirtualEntry GetEntry(VirtualPath virtualPath);
    IVirtualFile GetFile(VirtualPath virtualPath);
    IVirtualDirectory GetDirectory(VirtualPath virtualPath);

    IEnumerable<IVirtualEntry> GetEntries(VirtualPath virtualPath, bool recursive = false);
    IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath);
    IEnumerable<IVirtualFile> GetFilesRecursive(VirtualPath virtualPath);
    IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath);
}
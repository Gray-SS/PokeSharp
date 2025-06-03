using System.Diagnostics;
using PokeSharp.Assets.VFS.Extensions;
using PokeSharp.Assets.VFS.Volumes;

namespace PokeSharp.Assets.VFS;

public sealed class VirtualDirectory : VirtualEntry, IVirtualDirectory
{
    public bool IsRoot => Path.IsRoot;

    public VirtualDirectory(IVirtualVolume volume, VirtualPath path) : base(volume, path)
    {
        Debug.Assert(path.IsDirectory, "Provided path must represent a directory");
    }

    public IVirtualFile CreateFile(string fileName, bool overwrite = false)
    {
        VirtualPath virtualPath = Path.Combine(fileName);
        return Volume.AsWriteable().CreateFile(virtualPath, overwrite);
    }

    public IVirtualDirectory CreateDirectory(string dirName)
    {
        VirtualPath virtualPath = Path.Combine($"{dirName}/");
        return Volume.AsWriteable().CreateDirectory(virtualPath);
    }

    public bool EntryExists(string entryName)
    {
        VirtualPath virtualPath = Path.Combine(entryName);
        return Volume.AsFetchable().EntryExists(virtualPath);
    }

    public bool FileExists(string fileName)
    {
        VirtualPath virtualPath = Path.Combine(fileName);
        return Volume.AsFetchable().FileExists(virtualPath);
    }

    public bool DirectoryExists(string dirName)
    {
        VirtualPath virtualPath = Path.Combine(dirName + "/");
        return Volume.AsFetchable().DirectoryExists(virtualPath);
    }

    public IVirtualDirectory GetDirectory(string dirName)
    {
        VirtualPath virtualPath = Path.Combine(dirName + "/");
        return Volume.AsFetchable().GetDirectory(virtualPath);
    }

    public IVirtualFile GetFile(string fileName)
    {
        Debug.Assert(!fileName.EndsWith('/'), "The fileName must not end with a trailing '/'");

        VirtualPath virtualPath = Path.Combine(fileName);
        return Volume.AsFetchable().GetFile(virtualPath);
    }

    public IEnumerable<IVirtualDirectory> GetDirectories()
    {
        return Volume.AsFetchable().GetDirectories(Path);
    }

    public IEnumerable<IVirtualFile> GetFiles()
    {
        return Volume.AsFetchable().GetFiles(Path);
    }
}
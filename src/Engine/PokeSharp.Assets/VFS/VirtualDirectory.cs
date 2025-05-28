namespace PokeSharp.Assets.VFS;

public sealed class VirtualDirectory : VirtualEntry, IVirtualDirectory
{
    public VirtualDirectory(IVirtualFileSystemProvider provider, VirtualPath path) : base(provider, path)
    {
    }

    public IVirtualFile CreateFile(string fileName, bool overwrite = false)
    {
        VirtualPath virtualPath = Path.Combine(fileName);
        return Provider.CreateFile(virtualPath, overwrite);
    }

    public IVirtualDirectory CreateDirectory(string dirName)
    {
        VirtualPath virtualPath = Path.Combine($"{dirName}/");
        return Provider.CreateDirectory(virtualPath);
    }

    public bool FileExists(string fileName)
    {
        // TODO: Need to actually check if it's a directory or a file that exists, we're just checking if the path exists rn
        VirtualPath virtualPath = Path.Combine(fileName);
        return Provider.Exists(virtualPath);
    }

    public bool DirectoryExists(string dirName)
    {
        // TODO: Need to actually check if it's a directory or a file that exists, we're just checking if the path exists rn
        VirtualPath virtualPath = Path.Combine(dirName + "/");
        return Provider.Exists(virtualPath);
    }

    public IVirtualDirectory GetDirectory(string dirName)
    {
        VirtualPath virtualPath = Path.Combine(dirName + "/");
        return Provider.GetDirectory(virtualPath);
    }

    public IVirtualFile GetFile(string fileName)
    {
        VirtualPath virtualPath = Path.Combine(fileName);
        return Provider.GetFile(virtualPath);
    }

    public IEnumerable<IVirtualDirectory> GetDirectories()
    {
        return Provider.GetDirectories(Path);
    }

    public IEnumerable<IVirtualFile> GetFiles()
    {
        return Provider.GetFiles(Path);
    }
}
namespace PokeSharp.Assets.VFS;

public sealed class FileSystemProvider : IVirtualFileSystemProvider
{
    public string Name => "File System";
    public string RootPath { get; }
    public bool IsReadOnly => false;

    public IVirtualDirectory RootDir { get; }

    public FileSystemProvider(string rootPath)
    {
        if (!Directory.Exists(rootPath))
            throw new ArgumentException($"No directory was found at path: '{rootPath}'");

        RootPath = Path.GetFullPath(rootPath);
        RootDir = new VirtualDirectory(this, null, GetDirectoryName(rootPath));
    }

    private string GetPhysicalPath(string virtualPath)
    {
        var normalizedPath = virtualPath.Replace('/', Path.DirectorySeparatorChar);
        string physicalPath = Path.Combine(RootPath, normalizedPath.TrimStart(Path.DirectorySeparatorChar));

        return physicalPath;
    }

    public IVirtualFile? GetFile(string virtualPath)
    {
        string physicalPath = GetPhysicalPath(virtualPath);
        if (!File.Exists(physicalPath))
            return null;

        var parentDirPath = Path.GetDirectoryName(virtualPath);
        IVirtualDirectory? parent = parentDirPath != null ? GetDirectory(parentDirPath) : null;

        string fileName = Path.GetFileName(physicalPath);
        return new VirtualFile(this, parent, fileName);
    }

    public IVirtualDirectory? GetDirectory(string virtualPath)
    {
        string physicalPath = GetPhysicalPath(virtualPath);
        if (!Directory.Exists(physicalPath))
            return null;

        string dirName = GetDirectoryName(physicalPath);

        // TODO: Make sure this works
        var parentDirPath = Path.GetDirectoryName(virtualPath);
        IVirtualDirectory? parent = parentDirPath != null ? GetDirectory(parentDirPath) : null;

        return new VirtualDirectory(this, parent, dirName);
    }

    public IVirtualFile CreateFile(string virtualPath, bool overwrite)
    {
        string physicalPath = GetPhysicalPath(virtualPath);
        if (!overwrite && Path.Exists(physicalPath))
            throw new InvalidOperationException($"The path '{virtualPath}' already contains a file or a folder.");

        using var _ = File.Create(physicalPath);

        var parentDirPath = Path.GetDirectoryName(virtualPath);
        IVirtualDirectory? parent = parentDirPath != null ? GetDirectory(parentDirPath) : null;

        string fileName = Path.GetFileName(virtualPath);
        return new VirtualFile(this, parent, fileName);
    }

    public IVirtualDirectory CreateDirectory(string virtualPath)
    {
        string physicalPath = GetPhysicalPath(virtualPath);
        if (File.Exists(physicalPath))
            throw new InvalidOperationException($"The path '{virtualPath}' .");

        System.Console.WriteLine($"Create directory at: {physicalPath}");
        Directory.CreateDirectory(physicalPath);

        var parentDirPath = Path.GetDirectoryName(virtualPath);
        IVirtualDirectory? parent = parentDirPath != null ? GetDirectory(parentDirPath) : null;
        System.Console.WriteLine($"Parent dir path: {parentDirPath}");

        string dirName = GetDirectoryName(physicalPath);
        return new VirtualDirectory(this, parent, dirName);
    }

    public StreamWriter OpenWrite(string virtualPath)
    {
        string physicalPath = GetPhysicalPath(virtualPath);
        if (!File.Exists(physicalPath))
            throw new FileNotFoundException($"File at virtual path '{virtualPath}' doesn't exists.");

        FileStream stream = File.OpenWrite(physicalPath);
        return new StreamWriter(stream);
    }

    public StreamReader OpenRead(string virtualPath)
    {
        string physicalPath = GetPhysicalPath(virtualPath);
        if (!File.Exists(physicalPath))
            throw new FileNotFoundException($"File at virtual path '{virtualPath}' doesn't exists.");

        FileStream stream = File.OpenRead(physicalPath);
        return new StreamReader(stream);
    }

    private static string GetDirectoryName(string physicalPath)
    {
        return Path.GetFileName(physicalPath.TrimEnd(Path.DirectorySeparatorChar));
    }
}
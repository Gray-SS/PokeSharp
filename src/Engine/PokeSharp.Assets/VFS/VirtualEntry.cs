namespace PokeSharp.Assets.VFS;

public abstract class VirtualEntry : IVirtualEntry
{
    public string Name { get; }

    public string VirtualPath { get; }

    public IVirtualDirectory? ParentDirectory { get; }

    public bool IsReadOnly => Provider.IsReadOnly;

    public abstract bool IsFile { get; }

    public abstract bool IsDirectory { get; }

    public IVirtualFileSystemProvider Provider { get; }

    public VirtualEntry(IVirtualFileSystemProvider provider, IVirtualDirectory? parentDir, string name)
    {
        Name = name;
        Provider = provider;
        ParentDirectory = parentDir;

        if (parentDir == null)
        {
            if (!IsDirectory)
                throw new InvalidOperationException($"A virtual file cannot be created with a null parent directory: '{name}'");

            VirtualPath = string.Empty;
        }
        else
        {
            VirtualPath = $"{parentDir.VirtualPath}/{name}";
            if (IsDirectory)
                VirtualPath += "/";
        }
    }

    protected void EnsureWritePermissions()
    {
        if (IsReadOnly)
            throw new UnauthorizedAccessException($"You're unauthorized to perform this action. The provider is read-only.");
    }
}
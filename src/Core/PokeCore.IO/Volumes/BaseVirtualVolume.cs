namespace PokeCore.IO.Volumes;

public abstract class BaseVirtualVolume : IVirtualVolume, IFetchableVolume
{
    public string Id { get; }
    public string Scheme { get; }
    public string DisplayName { get; }
    public VirtualPath RootPath { get; }
    public IVirtualDirectory RootDirectory { get; }

    public bool IsReadOnly => !CanWrite;
    public VolumeAccess AuthorizedAccesses { get; }

    public bool CanFetch => HasAccess(VolumeAccess.Fetch);
    public bool CanRead => HasAccess(VolumeAccess.Read);
    public bool CanWrite => HasAccess(VolumeAccess.Write);
    public bool CanWatch => HasAccess(VolumeAccess.Watch);

    public BaseVirtualVolume(string id, string scheme, string displayName)
    {
        Id = id;
        Scheme = scheme;
        DisplayName = displayName;
        RootPath = VirtualPath.BuildRoot(scheme);
        RootDirectory = new VirtualDirectory(this, RootPath);

        AuthorizedAccesses = VolumeAccess.Fetch;
        if (this is IReadableVolume) AuthorizedAccesses |= VolumeAccess.Read;
        if (this is IWritableVolume) AuthorizedAccesses |= VolumeAccess.Write;
        if (this is IWatchableVolume) AuthorizedAccesses |= VolumeAccess.Watch;

        Configure();
    }

    protected virtual void Configure()
    {
    }

    public bool HasAccess(VolumeAccess access)
    {
        return (AuthorizedAccesses & access) != 0;
    }

    public abstract bool DirectoryExists(VirtualPath virtualPath);
    public abstract bool EntryExists(VirtualPath virtualPath);
    public abstract bool FileExists(VirtualPath virtualPath);
    public abstract IEnumerable<IVirtualDirectory> GetDirectories(VirtualPath virtualPath);
    public abstract IVirtualDirectory GetDirectory(VirtualPath virtualPath);

    public virtual IEnumerable<IVirtualFile> GetFilesRecursive(VirtualPath virtualPath)
    {
        foreach (IVirtualDirectory directory in GetDirectories(virtualPath))
        {
            IEnumerable<IVirtualFile> files = GetFilesRecursive(directory.Path);
            foreach (IVirtualFile file in files)
                yield return file;
        }

        foreach (IVirtualFile file in GetFiles(virtualPath))
            yield return file;
    }

    public virtual IEnumerable<IVirtualEntry> GetEntries(VirtualPath virtualPath, bool isRecursive)
    {
        foreach (IVirtualDirectory directory in GetDirectories(virtualPath))
        {
            yield return directory;

            if (isRecursive)
            {
                IEnumerable<IVirtualEntry> entries = GetEntries(directory.Path, isRecursive: true);
                foreach (IVirtualEntry entry in entries)
                    yield return entry;
            }
        }

        foreach (IVirtualFile file in GetFiles(virtualPath))
        {
            yield return file;
        }
    }

    public virtual IVirtualEntry GetEntry(VirtualPath virtualPath)
    {
        if (virtualPath.IsFile) return GetFile(virtualPath);

        return GetDirectory(virtualPath);
    }


    public abstract IVirtualFile GetFile(VirtualPath virtualPath);
    public abstract IEnumerable<IVirtualFile> GetFiles(VirtualPath virtualPath);
}
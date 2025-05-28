namespace PokeSharp.Assets.VFS;

public sealed class VolumeInfo
{
    public string Scheme { get; }
    public string DisplayName { get; }
    public string VolumeType { get; }
    public VirtualPath RootPath { get; }
    public FileSystemAccess Access { get; }

    public VolumeInfo(string scheme, string displayName, string volumeType, FileSystemAccess access)
    {
        ArgumentNullException.ThrowIfNull(scheme);
        ArgumentNullException.ThrowIfNull(displayName);
        ArgumentNullException.ThrowIfNull(volumeType);

        Scheme = scheme;
        Access = access;
        DisplayName = displayName;
        VolumeType = volumeType;
        RootPath = VirtualPath.Root(scheme);
    }

    public bool HasAccessTo(FileSystemAccess access)
    {
        return Access.HasFlag(access);
    }
}
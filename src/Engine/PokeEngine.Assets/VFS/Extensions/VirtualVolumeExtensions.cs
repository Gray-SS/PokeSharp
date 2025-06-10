using PokeEngine.Assets.VFS.Volumes;

namespace PokeEngine.Assets.VFS.Extensions;

public static class VirtualVolumeExtensions
{
    public static bool CanWrite(this VolumeAccess access)
    {
        return (access & VolumeAccess.Write) != 0;
    }

    public static bool CanRead(this VolumeAccess access)
    {
        return (access & VolumeAccess.Read) != 0;
    }

    public static bool CanFetch(this VolumeAccess access)
    {
        return (access & VolumeAccess.Fetch) != 0;
    }

    public static bool CanWatch(this VolumeAccess access)
    {
        return (access & VolumeAccess.Watch) != 0;
    }

    public static IReadableVolume AsReadable(this IVirtualVolume volume)
    {
        if (volume is not IReadableVolume readable)
            throw new UnauthorizedAccessException($"Unauthorized access requested for volume '{volume.DisplayName}'. Volume is not readable.");

        return readable;
    }

    public static IFetchableVolume AsFetchable(this IVirtualVolume volume)
    {
        if (volume is not IFetchableVolume fetchable)
            throw new UnauthorizedAccessException($"Unauthorized access requested for volume '{volume.DisplayName}'. Volume is not fetchable.");

        return fetchable;
    }

    public static IWritableVolume AsWriteable(this IVirtualVolume volume)
    {
        if (volume is not IWritableVolume writeable)
            throw new UnauthorizedAccessException($"Unauthorized access requested for volume '{volume.DisplayName}'. Volume is not writeable.");

        return writeable;
    }
}
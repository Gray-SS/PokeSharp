namespace PokeEngine.Assets.VFS.Volumes;

public interface IReadableVolume
{
    Stream OpenRead(VirtualPath virtualPath);
    byte[] ReadBytes(VirtualPath virtualPath);
}
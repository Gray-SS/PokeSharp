namespace PokeSharp.Assets.VFS.Volumes;

public interface IReadableVolume
{
    Stream OpenRead(VirtualPath virtualPath);
    byte[] ReadBytes(VirtualPath virtualPath);
}
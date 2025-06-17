namespace PokeCore.IO.Volumes;

public interface IReadableVolume
{
    Stream OpenRead(VirtualPath virtualPath);
    byte[] ReadBytes(VirtualPath virtualPath);
}
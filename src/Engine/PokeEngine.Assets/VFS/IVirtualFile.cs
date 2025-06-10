namespace PokeEngine.Assets.VFS;

public interface IVirtualFile : IVirtualEntry, IEquatable<IVirtualEntry>
{
    Stream OpenRead();
    Stream OpenWrite();
    byte[] ReadBytes();
}
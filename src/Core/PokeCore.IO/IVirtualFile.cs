namespace PokeCore.IO;

public interface IVirtualFile : IVirtualEntry, IEquatable<IVirtualEntry>
{
    Stream OpenRead();
    Stream OpenWrite();
    byte[] ReadBytes();
}
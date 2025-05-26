namespace PokeSharp.Assets.VFS;

public interface IVirtualFile : IVirtualEntry
{
    StreamReader OpenRead();
    StreamWriter OpenWrite();
}
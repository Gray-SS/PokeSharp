namespace PokeEngine.Assets.VFS;

[Flags]
public enum FileSystemAccess : byte
{
    None = 0,
    Read = 1 << 0,
    Write = 1 << 1,
}
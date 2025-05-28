namespace PokeSharp.Assets.VFS;

[Flags]
public enum FileSystemAccess
{
    None = 0,
    Read = 1 << 0,
    Write = 1 << 1,
    All = Read | Write,
}
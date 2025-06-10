namespace PokeEngine.Assets.VFS.Volumes;

[Flags]
public enum VolumeAccess
{
    None = 0,
    Fetch = 1 << 0,
    Read = 1 << 1,
    Write = 1 << 2,
    Watch = 1 << 3,
    All = Fetch | Read | Write | Watch,
}
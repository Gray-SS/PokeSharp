using PokeCore.IO;

namespace PokeEngine.ROM;

public abstract class RomEntry
{
    public string Name => Path.Name;
    public VirtualPath Path { get; }

    public RomEntry(VirtualPath path)
    {
        Path = path;
    }
}
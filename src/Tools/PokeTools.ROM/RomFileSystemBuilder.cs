namespace PokeTools.ROM;

public abstract class RomFileSystemBuilder
{
    public Rom Rom { get; }

    public RomFileSystemBuilder(Rom rom)
    {
        Rom = rom;
    }

    public abstract void Build(RomDirectory root);
}
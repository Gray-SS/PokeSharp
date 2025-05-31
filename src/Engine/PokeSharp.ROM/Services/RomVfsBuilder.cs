namespace PokeSharp.ROM.Services;

public abstract class RomVfsBuilder
{
    public Rom Rom { get; }

    public RomVfsBuilder(Rom rom)
    {
        Rom = rom;
    }

    public abstract void Build(RomDirectory root);
}
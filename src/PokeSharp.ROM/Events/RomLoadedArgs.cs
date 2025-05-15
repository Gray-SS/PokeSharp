namespace PokeSharp.ROM.Events;

public sealed class RomLoadedArgs : EventArgs
{
    public PokemonRom LoadedRom { get; }

    public RomLoadedArgs(PokemonRom loadedRom)
    {
        LoadedRom = loadedRom;
    }
}
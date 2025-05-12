using System.Globalization;

namespace Pokemon.DesktopGL.ROM;

public sealed class PokemonRom : IPokemonRomProvider
{
    public RomInfo Info { get; }
    public byte[] Data { get; }

    private readonly IPokemonRomProvider _provider;

    public PokemonRom(RomInfo info, byte[] data, IPokemonRomProvider provider)
    {
        Info = info;
        Data = data;

        _provider = provider;
    }

    public string GetPokemonName(int index)
    {
        string pokemonName = _provider.GetPokemonName(index);
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pokemonName);
    }
}
namespace Pokemon.DesktopGL.ROM;

public interface IPokemonRomProvider
{
    byte[] Data { get; }

    string GetPokemonName(int index);
}
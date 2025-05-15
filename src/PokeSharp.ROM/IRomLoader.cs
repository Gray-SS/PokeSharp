namespace PokeSharp.ROM;

public interface IRomLoader
{
    RomInfo LoadRom(byte[] romData);
}
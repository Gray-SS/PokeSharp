namespace Pokemon.DesktopGL.ROM;

public interface IRomLoader
{
    RomInfo LoadRom(byte[] romData);
}
using System.Text;

namespace PokeSharp.ROM.Platforms.GBA;

public sealed class GbaRomLoader : IRomLoader
{
    public RomInfo LoadRom(byte[] romData)
    {
        string gameCode = ExtractGameCode(romData);
        string language = gameCode[3] switch
        {
            'J' => "Japanese",
            'P' => "Europe/Elsewhere",
            'F' => "French",
            'S' => "Spanish",
            'E' => "USA/English",
            'D' => "German",
            'I' => "Italian",
            _ => "Unknown"
        };

        string makerCode = ExtractMakerCode(romData);
        string maker = makerCode == "01" ? "Nintendo" : "Unknown";

        return new RomInfo
        {
            GameTitle = ExtractGameTitle(romData),
            GameCode = gameCode,
            MakerCode = makerCode,
            Maker = maker,
            Language = language,
            Version = ExtractVersion(romData),
            Platform = RomPlatform.GBA
        };
    }

    private static string ExtractGameTitle(byte[] romData)
    {
        if (romData.Length <= 0xA0 + 12)
            throw new FormatException("The GBA ROM isn't in the correct format.");

        return Encoding.ASCII.GetString(romData, 0xA0, 12);
    }

    private static string ExtractGameCode(byte[] romData)
    {
        if (romData.Length <= 0xAC + 4)
            throw new FormatException("The GBA ROM isn't in the correct format.");

        return Encoding.ASCII.GetString(romData, 0xAC, 4);
    }

    private static string ExtractMakerCode(byte[] romData)
    {
        if (romData.Length <= 0xB0 + 2)
            throw new FormatException("The GBA ROM isn't in the correct format.");

        return Encoding.ASCII.GetString(romData, 0xB0, 2);
    }

    private static int ExtractVersion(byte[] romData)
    {
        if (romData.Length <= 0xBC)
            throw new FormatException("The GBA ROM isn't in the correct format.");

        return romData[0xBC];
    }
}
using System;
using System.Text;

namespace Pokemon.DesktopGL.ROM.GBA;

public sealed class GbaRomLoader : IRomLoader
{
    public RomInfo LoadRom(byte[] romData)
    {
        string gameCode = LoadGameCode(romData);
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

        string makerCode = LoadMakerCode(romData);
        string maker = makerCode == "01" ? "Nintendo" : "Unknown";

        return new RomInfo
        {
            GameTitle = LoadGameTitle(romData),
            GameCode = gameCode,
            MakerCode = makerCode,
            Maker = maker,
            Language = language,
            Platform = RomPlatform.GBA
        };
    }

    private static string LoadGameTitle(byte[] romData)
    {
        if (romData.Length <= 0xA0 + 12)
            throw new FormatException("The GBA ROM isn't in the correct format.");

        return Encoding.ASCII.GetString(romData, 0xA0, 12);
    }

    private static string LoadGameCode(byte[] romData)
    {
        if (romData.Length <= 0xAC + 4)
            throw new FormatException("The GBA ROM isn't in the correct format.");

        return Encoding.ASCII.GetString(romData, 0xAC, 4);
    }

    private static string LoadMakerCode(byte[] romData)
    {
        if (romData.Length <= 0xB0 + 2)
            throw new FormatException("The GBA ROM isn't in the correct format.");

        return Encoding.ASCII.GetString(romData, 0xB0, 2);
    }
}
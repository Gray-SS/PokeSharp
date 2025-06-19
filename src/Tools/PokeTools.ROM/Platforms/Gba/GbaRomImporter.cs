using System.Text;
using PokeEngine.Assets;
using PokeEngine.Assets.Exceptions;
using PokeCore.IO;

namespace PokeEngine.ROM.Platforms.Gba;

public sealed class GbaRomImporter : AssetImporter<RomInfo>
{
    public override Type ProcessorType => typeof(GbaRomProcessor);
    public override string SupportedExtensions => ".gba";

    public override RomInfo Import(IVirtualFile file)
    {
        byte[] rawData = file.ReadBytes();

        if (!IsValidGbaRom(rawData))
            throw new AssetImporterException($"The provided gba at path '{file.Path}' is not valid. Please make sure it's a valid GBA.");

        string gameCode = ExtractGameCode(rawData);
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

        string makerCode = ExtractMakerCode(rawData);
        string maker = makerCode == "01" ? "Nintendo" : "Unknown";

        var romInfo = new RomInfo
        {
            RawData = rawData,
            GameTitle = ExtractGameTitle(rawData),
            GameCode = gameCode,
            MakerCode = makerCode,
            Maker = maker,
            Language = language,
            Version = ExtractVersion(rawData),
            Platform = RomPlatform.GBA
        };

        if (!romInfo.IsPokemonROM)
        {
            throw new AssetImporterException("The imported ROM is not a pokémon ROM. Please make sure you provided a valid pokémon ROM.");
        }

        return romInfo;
    }

    private static string ExtractGameTitle(byte[] rawData)
    {
        if (rawData.Length <= 0xA0 + 12)
            throw new AssetImporterException("The GBA ROM isn't in the correct format.");

        return Encoding.ASCII.GetString(rawData, 0xA0, 12);
    }

    private static string ExtractGameCode(byte[] rawData)
    {
        if (rawData.Length <= 0xAC + 4)
            throw new AssetImporterException("The GBA ROM isn't in the correct format.");

        return Encoding.ASCII.GetString(rawData, 0xAC, 4);
    }

    private static string ExtractMakerCode(byte[] rawData)
    {
        if (rawData.Length <= 0xB0 + 2)
            throw new AssetImporterException("The GBA ROM isn't in the correct format.");

        return Encoding.ASCII.GetString(rawData, 0xB0, 2);
    }

    private static int ExtractVersion(byte[] rawData)
    {
        if (rawData.Length <= 0xBC)
            throw new AssetImporterException("The GBA ROM isn't in the correct format.");

        return rawData[0xBC];
    }

    // TODO: Probably add more validation to verify if it's indeed a valid GBA ROM.
    private static bool IsValidGbaRom(ReadOnlySpan<byte> rawData)
    {
        if (rawData.Length <= 0xB2)
            return false;

        return rawData[0xB2] == 0x96;
    }
}
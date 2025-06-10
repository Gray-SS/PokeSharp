using PokeEngine.Core.Exceptions;

namespace PokeEngine.ROM;

public sealed class RomPlatform
{
    public required int PlatformId { get; init; }
    public required string Name { get; init; }
    public required string ShortName { get; init; }
    public required int PointerSizeBits { get; init; }
    public required string CpuArchitecture { get; init; }
    public required bool IsLitteEndian { get; init; }
    public required bool SupportsFileSystem { get; init; }
    public required string[] SupportedExtensions { get; init; }

    public static readonly RomPlatform GBA = new RomPlatform
    {
        PlatformId = 1,
        Name = "Game Boy Advance",
        ShortName = "GBA",
        PointerSizeBits = 32,
        CpuArchitecture = "ARM7TDMI",
        IsLitteEndian = true,
        SupportsFileSystem = false,
        SupportedExtensions = [".gba"],
    };

    private static readonly Dictionary<int, RomPlatform> _platforms = new()
    {
        { GBA.PlatformId, GBA }
    };

    private RomPlatform()
    {
    }

    public static RomPlatform GetPlatformById(int id)
    {
        if (!_platforms.TryGetValue(id, out RomPlatform? platform))
        {
            throw new EngineException($"Unknown ROM platform ID: '{id}'. Ensure the id is correct.");
        }

        return platform;
    }

    public override string ToString()
    {
        return $"{Name} {ShortName}";
    }
}
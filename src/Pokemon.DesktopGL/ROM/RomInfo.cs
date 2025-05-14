using System;

namespace Pokemon.DesktopGL.ROM;

public sealed class RomInfo
{
    public required string GameTitle { get; init; }

    public required string GameCode { get; init; }

    public required RomPlatform Platform { get; init; }

    public required string Maker { get; init; }

    public required string Language { get; init; }

    public required string MakerCode { get; init; }

    public required int Version { get; init; }

    public string VersionText => $"v1.{Version}";

    public string UniqueGameCode => $"{Platform}_{GameCode}_{VersionText}";

    public bool IsPokemonROM => GameTitle.Contains("POKEMON", StringComparison.OrdinalIgnoreCase);

    public override string ToString()
    {
        return $"{GameTitle} {VersionText} by {Maker} ({Language})";
    }
}
using System.Diagnostics.CodeAnalysis;
using PokeSharp.ROM.Platforms.Gba;

namespace PokeSharp.ROM.Services;

public interface IGbaConfigProvider
{
    IReadOnlyCollection<GbaConfig> Configs { get; }

    GbaConfig GetConfig(string gameCode);
    bool TryGetConfig(string gameCode, [NotNullWhen(true)] out GbaConfig? config);
}
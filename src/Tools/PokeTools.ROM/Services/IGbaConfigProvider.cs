using System.Diagnostics.CodeAnalysis;
using PokeTools.ROM.Platforms.Gba;

namespace PokeTools.ROM.Services;

public interface IGbaConfigProvider
{
    IReadOnlyCollection<GbaConfig> Configs { get; }

    GbaConfig GetConfig(string gameCode);
    bool TryGetConfig(string gameCode, [NotNullWhen(true)] out GbaConfig? config);
}
using System.Diagnostics.CodeAnalysis;
using PokeEngine.ROM.Platforms.Gba;

namespace PokeEngine.ROM.Services;

public interface IGbaConfigProvider
{
    IReadOnlyCollection<GbaConfig> Configs { get; }

    GbaConfig GetConfig(string gameCode);
    bool TryGetConfig(string gameCode, [NotNullWhen(true)] out GbaConfig? config);
}
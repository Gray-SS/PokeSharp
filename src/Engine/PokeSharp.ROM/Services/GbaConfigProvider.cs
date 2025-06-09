using System.Diagnostics.CodeAnalysis;
using PokeSharp.Engine.Core.Exceptions;
using PokeSharp.ROM.Platforms.Gba;

namespace PokeSharp.ROM.Services;

public sealed class GbaConfigProvider : IGbaConfigProvider
{
    public IReadOnlyCollection<GbaConfig> Configs => _configsMap.Values;

    private readonly Dictionary<string, GbaConfig> _configsMap;

    public GbaConfigProvider(string yamlPath)
    {
        var yaml = File.ReadAllText(yamlPath);
        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();

        var configurations = deserializer.Deserialize<GbaConfig[]>(yaml);
        _configsMap = configurations.ToDictionary(c => c.GameCode);
    }

    public GbaConfig GetConfig(string gameCode)
    {
        if (!TryGetConfig(gameCode, out GbaConfig? config))
        {
            throw new EngineException($"""
                No configuration with '{gameCode}' was found.
                Make sure the game is supported or use the '{nameof(TryGetConfig)}' method instead.
            """);
        }

        return config;
    }

    public bool TryGetConfig(string gameCode, [NotNullWhen(true)] out GbaConfig? config)
    {
        return _configsMap.TryGetValue(gameCode, out config);
    }
}
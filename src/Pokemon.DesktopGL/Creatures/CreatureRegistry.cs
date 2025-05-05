using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Pokemon.DesktopGL.Core.Managers;

namespace Pokemon.DesktopGL.Creatures;

public sealed class CreatureRegistry
{
    private readonly AssetsManager _assetsManager;
    private readonly Dictionary<string, CreatureData> _creatures;
    private readonly Dictionary<string, CreatureZone> _zones;

    public CreatureRegistry(AssetsManager assetsManager)
    {
        _assetsManager = assetsManager;
        _creatures = new Dictionary<string, CreatureData>();
        _zones = new Dictionary<string, CreatureZone>();
    }

    public void Load()
    {
        LoadCreatures("Content/Data/Pokemons/pokemons.json");
        LoadZones("Content/Data/Pokemons/zones.json");
    }

    public CreatureData GetData(string id)
        => _creatures[id];

    public CreatureZone GetZone(string id)
        => _zones[id];

    private void LoadZones(string path)
    {
        string text = File.ReadAllText(path);
        CreatureZone[] zones = JsonSerializer.Deserialize<CreatureZone[]>(text);

        foreach (CreatureZone zone in zones)
        {
            float totalProb = 0.0f;

            foreach (CreatureSpawnEntry spawnEntry in zone.Creatures)
            {
                totalProb += spawnEntry.SpawnRate;
                spawnEntry.CreatureData = GetData(spawnEntry.CreatureId);
            }

            const float tolerance = 0.0001f;
            if (Math.Abs(totalProb - 1.0f) > tolerance)
            {
                throw new FormatException($"La zone n'est pas correctement formatée. La somme des probabilités doit être égale à 1 (valeur actuelle: {totalProb})");
            }

            _zones.Add(zone.Id, zone);
        }
    }

    private void LoadCreatures(string path)
    {
        string text = File.ReadAllText(path);
        CreatureData[] creatures = JsonSerializer.Deserialize<CreatureData[]>(text);

        foreach (CreatureData data in creatures)
        {
            data.BackSprite = _assetsManager.LoadSprite(data.BackSpritePath);
            data.FrontSprite = _assetsManager.LoadSprite(data.FrontSpritePath);

            _creatures[data.Id] = data;
        }
    }
}
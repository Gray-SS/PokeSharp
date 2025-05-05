using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pokemon.DesktopGL.Creatures;

public sealed class CreatureZone
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("creatures")]
    public List<CreatureSpawnEntry> Creatures { get; init; }

    public Creature Spawn()
    {
        float prob = Random.Shared.NextSingle();
        float totalProb = 0.0f;

        foreach (CreatureSpawnEntry entry in Creatures)
        {
            totalProb += entry.SpawnRate;

            if (prob <= totalProb)
            {
                var level = Random.Shared.Next(entry.MinLevel, entry.MaxLevel + 1);
                return entry.CreatureData.Create(level);
            }
        }

        return null;
    }
}
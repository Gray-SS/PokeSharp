using System.Text.Json.Serialization;

namespace Pokemon.DesktopGL.Creatures;

public sealed class CreatureSpawnEntry
{
    [JsonPropertyName("creatureId")]
    public string CreatureId { get; init; }

    [JsonIgnore]
    public CreatureData CreatureData { get; set; }

    [JsonPropertyName("spawnRate")]
    public float SpawnRate { get; init; }

    [JsonPropertyName("minLevel")]
    public int MinLevel { get; init; }

    [JsonPropertyName("maxLevel")]
    public int MaxLevel { get; init; }
}
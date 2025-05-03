using System.Text.Json.Serialization;

namespace Pokemon.DesktopGL.World;

public sealed class EntityDefinition
{
    [JsonPropertyName("type")]
    public required EntityType Type { get; init; }

    [JsonPropertyName("spawnCol")]
    public required int SpawnCol { get; init; }

    [JsonPropertyName("spawnRow")]
    public required int SpawnRow { get; init; }

    [JsonPropertyName("characterId")]
    public string CharacterId { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("dialogues")]
    public string[] Dialogues { get; init; }
}
using System.Text.Json.Serialization;

namespace Pokemon.DesktopGL.NPCs;

public sealed class NPCData
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("dialogues")]
    public required string[] Dialogues { get; init; }
}
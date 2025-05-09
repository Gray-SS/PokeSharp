using System.Text.Json.Serialization;
using Pokemon.DesktopGL.Creatures;

namespace Pokemon.DesktopGL.Moves;

public sealed class MoveData
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("PP")]
    public required int PP { get; init; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CreatureType Type { get; init; }

    [JsonPropertyName("category")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required MoveCategory Category { get; init; }

    [JsonPropertyName("accuracy")]
    public int Accuracy { get; init; }

    [JsonPropertyName("power")]
    public int Power { get; init; }
}
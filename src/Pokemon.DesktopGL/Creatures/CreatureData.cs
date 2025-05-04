using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using Pokemon.DesktopGL.Core.Graphics;

namespace Pokemon.DesktopGL.Creatures;

public sealed class CreatureData
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("baseHP")]
    public required int BaseHP { get; init; }

    [JsonPropertyName("baseAttack")]
    public required int BaseAttack { get; init; }

    [JsonPropertyName("baseDefense")]
    public required int BaseDefense { get; init; }

    [JsonPropertyName("frontSprite")]
    public required string FrontSpritePath { get; init; }

    [JsonPropertyName("backSprite")]
    public required string BackSpritePath { get; init; }

    [JsonIgnore]
    public Sprite FrontSprite { get; set; }

    [JsonIgnore]
    public Sprite BackSprite { get; set; }

    public Creature Create(int level)
        => new Creature(this, level);
}
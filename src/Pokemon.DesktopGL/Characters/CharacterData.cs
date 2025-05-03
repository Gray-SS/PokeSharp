using System.Text.Json.Serialization;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.Core.Graphics;

namespace Pokemon.DesktopGL.Characters;

public class CharacterData
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("spritesheetPath")]
    public required string SpritesheetPath { get; init; }

    [JsonIgnore]
    public AnimationPack IdleAnimations { get; set; }

    [JsonIgnore]
    public AnimationPack RunAnimations { get; set; }
}
using System.Text.Json.Serialization;

namespace Pokemon.DesktopGL.Creatures;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CreatureType
{
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy,
    None
}
using System.Text.Json.Serialization;

namespace Pokemon.DesktopGL.Creatures;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CreatureGrowthRate
{
    Erratic,
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Fluctuant
}
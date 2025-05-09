using System.Text.Json.Serialization;

namespace Pokemon.DesktopGL.Moves;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MoveCategory
{
    Physical,
    Special,
    Status
}
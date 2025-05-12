using System;
using System.Text.Json.Serialization;

namespace Pokemon.DesktopGL.Creatures;

public sealed class Stats
{
    public static readonly Stats Zero = new();

    [JsonPropertyName("hp")]
    public int HP { get; set; }

    [JsonPropertyName("attack")]
    public int Attack { get; set; }

    [JsonPropertyName("defense")]
    public int Defense { get; set; }

    [JsonPropertyName("spAtk")]
    public int SpAttack { get; set; }

    [JsonPropertyName("spDef")]
    public int SpDefense { get; set; }

    [JsonPropertyName("speed")]
    public int Speed { get; set; }

    public static Stats GenerateRandom()
    {
        var rnd = Random.Shared;
        return new Stats
        {
            HP = rnd.Next(0, 32),
            Attack = rnd.Next(0, 32),
            Defense = rnd.Next(0, 32),
            SpAttack = rnd.Next(0, 32),
            SpDefense = rnd.Next(0, 32),
            Speed = rnd.Next(0, 32),
        };
    }
}
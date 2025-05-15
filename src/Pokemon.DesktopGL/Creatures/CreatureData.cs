using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using PokeSharp.Engine.Graphics;
using Pokemon.DesktopGL.Moves;

namespace Pokemon.DesktopGL.Creatures;

public sealed class CreatureData
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("baseStats")]
    public required Stats BaseStats { get; init; }

    [JsonPropertyName("growthRate")]
    public required CreatureGrowthRate GrowthRate { get; init; }

    [JsonPropertyName("baseEXP")]
    public required int BaseEXP { get; init; }

    [JsonPropertyName("catchRate")]
    public required int CatchRate { get; init; }

    [JsonPropertyName("type1")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CreatureType Type1 { get; init; }

    [JsonPropertyName("type2")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CreatureType? Type2 { get; init; }

    [JsonPropertyName("learnableMoves")]
    public required Dictionary<int, string[]> LearnableMoveIds { get; init; }

    [JsonPropertyName("frontSprite")]
    public required string FrontSpritePath { get; init; }

    [JsonPropertyName("backSprite")]
    public required string BackSpritePath { get; init; }

    [JsonIgnore]
    public Sprite FrontSprite { get; set; }

    [JsonIgnore]
    public Sprite BackSprite { get; set; }

    [JsonIgnore]
    public IEnumerable<CreatureType> Types
    {
        get
        {
            yield return Type1;

            if (Type2.HasValue)
                yield return Type2.Value;
        }
    }

    [JsonIgnore]
    public Dictionary<int, MoveData[]> LearnableMoves { get; set; }

    public Creature CreateWild(int level)
    {
        Creature creature = new Creature(this, level);
        foreach (MoveData move in creature.Data.LearnableMoves.Where(x => x.Key <= level).OrderBy(x => x.Key).SelectMany(x => x.Value))
        {
            if (creature.Moves.Count == 4)
                break;

            creature.AddMove(move);
        }

        return creature;
    }
}
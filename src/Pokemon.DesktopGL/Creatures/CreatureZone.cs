using System;
using System.Linq;
using Pokemon.DesktopGL.Moves;

namespace Pokemon.DesktopGL.Creatures;

// Probably rename this class because it's directly related to the world
public sealed class CreatureZone
{
    public string Name { get; init; }
    public CreatureSpawnEntry[] Creatures { get; init; }

    public Creature SpawnWildCreature()
    {
        float prob = Random.Shared.NextSingle();
        float totalProb = 0.0f;

        foreach (CreatureSpawnEntry entry in Creatures)
        {
            totalProb += entry.SpawnRate;

            if (prob <= totalProb)
            {
                var level = Random.Shared.Next(entry.MinLevel, entry.MaxLevel + 1);

                Creature creature = entry.CreatureData.Create(level);
                foreach (MoveData move in creature.Data.LearnableMoves.Where(x => x.Key <= level).OrderBy(x => x.Key).SelectMany(x => x.Value))
                {
                    creature.AddMove(move);

                    if (creature.Moves.Count == 4)
                        break;
                }

                return creature;
            }
        }

        return null;
    }
}
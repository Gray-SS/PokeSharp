using System.Collections.Generic;
using System.Linq;
using Pokemon.DesktopGL.Creatures;

namespace Pokemon.DesktopGL.Players;

public sealed class PlayerData
{
    public string Name { get; set; }
    public bool CanFight => Creatures.Any(x => x.HP > 0);
    public List<Creature> Creatures { get; }

    public PlayerData()
    {
        Creatures = new List<Creature>(6);
    }

    public void AddCreature(Creature creature)
    {
        Creatures.Add(creature);
    }

    public void RemoveCreature(Creature creature)
    {
        Creatures.Remove(creature);
    }
}
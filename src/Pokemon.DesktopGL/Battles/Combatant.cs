using System.Collections.Generic;
using Pokemon.DesktopGL.Creatures;

namespace Pokemon.DesktopGL.Battles;

public sealed class Combatant
{
    public bool IsPlayer => Battle.Player == this;

    public Battle Battle { get; private set; }
    public Creature ActiveCreature => _creatures[_activeCreatureIndex];
    public IReadOnlyList<Creature> Creatures => _creatures;

    private int _activeCreatureIndex;
    private readonly List<Creature> _creatures;

    public Combatant(List<Creature> creatures)
    {
        _creatures = creatures;
    }

    public void BindBattle(Battle battle)
        => Battle = battle;
}
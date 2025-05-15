using System.Collections.Generic;
using System.Diagnostics;
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

    public void SwitchCreature(int creatureIndex)
    {
        Debug.Assert(creatureIndex < 0 || creatureIndex >= _creatures.Count, "Creature index was out of range when switching creature.");

        _activeCreatureIndex = creatureIndex;
    }

    public void BindBattle(Battle battle)
        => Battle = battle;
}
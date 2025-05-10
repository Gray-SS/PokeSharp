using System;
using System.Collections;

namespace Pokemon.DesktopGL.Battles;

public sealed class BattleContext
{
    public Battle Battle { get; }
    public Combatant Player => Battle.Player;
    public Combatant Opponent => Battle.Opponent;

    public Combatant Attacker { get; set; }
    public IBattleMove AttackerMove { get; set; }
    public Combatant Defender { get; set; }

    private readonly Func<string, IEnumerator> _write;

    public BattleContext(Battle battle,Func<string, IEnumerator> write)
    {
        Battle = battle;
        _write = write;
    }

    public IEnumerator Write(string text)
        => _write.Invoke(text);
}
using System;
using System.Collections;
using System.Linq;
using Pokemon.DesktopGL.Core.Coroutines;
using Pokemon.DesktopGL.Creatures;
using Pokemon.DesktopGL.Miscellaneous;
using Pokemon.DesktopGL.Moves;

namespace Pokemon.DesktopGL.Battles.Moves;

public class FightMove : IBattleMove
{
    public string Name => _move.Name;

    private float _effectiveness;
    private readonly MoveData _move;
    private readonly int _moveIndex;

    public FightMove(MoveData move, int moveIndex)
    {
        _move = move;
        _moveIndex = moveIndex;
    }

    public IEnumerator Before(BattleContext context)
    {
        MoveData move = _move;
        Creature attackerCreature = context.Attacker.ActiveCreature;

        yield return context.Write($"{attackerCreature.Data.Name} utilise {move.Name}!");
        yield return new WaitForSeconds(1.0f);
    }

    public IEnumerator After(BattleContext context)
    {
        if (_effectiveness == 0.0f)
            yield return context.Write("Ce n'est pas du tout efficace...");
        else if (_effectiveness == 0.5f)
            yield return context.Write("Ce n'est pas très efficace...");
        else if (_effectiveness > 1.0f)
            yield return context.Write("C'est super efficace!");

        yield return new WaitForSeconds(1.0f);
    }

    public IEnumerator Execute(BattleContext context)
    {
        var move = _move;
        var attackerCreature = context.Attacker.ActiveCreature;
        var targetCreature = context.Defender.ActiveCreature;

        int attackStat = move.Category == MoveCategory.Physical
            ? attackerCreature.Attack
            : attackerCreature.SpAtk;

        int defenseStat = move.Category == MoveCategory.Physical
            ? targetCreature.Defense
            : targetCreature.SpDef;

        float stab = attackerCreature.Data.Types.Contains(move.Type)
            ? 1.5f
            : 1.0f;

        _effectiveness = Utils.GetTypeEffectiveness(move.Type, targetCreature.Data.Type1, targetCreature.Data.Type2);
        float modifier = stab * _effectiveness * (Random.Shared.NextSingle() * 0.15f + 0.85f);

        float damage = ((2 * attackerCreature.Level / 5f + 2) * move.Power * (attackStat / (float)defenseStat) / 50 + 2) * modifier;

        targetCreature.TakeDamage((int)MathF.Floor(damage));
        yield return new WaitForSeconds(1.0f);
    }
}
using System;
using System.Collections;
using System.Linq;
using PokeSharp.Engine.Coroutines;
using Pokemon.DesktopGL.Creatures;
using Pokemon.DesktopGL.Miscellaneous;
using Pokemon.DesktopGL.Moves;

namespace Pokemon.DesktopGL.Battles.Moves;

public class FightMove : IBattleMove
{
    private float _effectiveness;
    private readonly MoveData _move;

    public FightMove(MoveData move)
    {
        _move = move;
    }

    public IEnumerator Before(BattleContext context)
    {
        MoveData move = _move;
        Creature attackerCreature = context.Attacker.ActiveCreature;

        yield return context.Write($"{attackerCreature.Data.Name} used {move.Name}!");
        yield return new WaitForSeconds(1.0f);
    }

    public IEnumerator After(BattleContext context)
    {
        if (_effectiveness == 0.0f)
            yield return context.Write("It has no effect...");
        else if (_effectiveness == 0.5f)
            yield return context.Write("It's not very effective...");
        else if (_effectiveness > 1.0f)
            yield return context.Write("It's super effective!");

        yield return new WaitForSeconds(1.0f);
    }

    public IEnumerator Execute(BattleContext context)
    {
        var move = _move;
        var attackerCreature = context.Attacker.ActiveCreature;
        var targetCreature = context.Defender.ActiveCreature;

        int attackStat = move.Category == MoveCategory.Physical
            ? attackerCreature.Attack
            : attackerCreature.SpAttack;

        int defenseStat = move.Category == MoveCategory.Physical
            ? targetCreature.Defense
            : targetCreature.SpDefense;

        float stab = attackerCreature.Data.Types.Contains(move.Type)
            ? 1.5f
            : 1.0f;

        _effectiveness = Utils.GetTypeEffectiveness(move.Type, targetCreature.Data.Type1, targetCreature.Data.Type2);
        float modifier = stab * _effectiveness * (Random.Shared.NextSingle() * 0.15f + 0.85f);

        int damage = (int)MathF.Floor((((2 * attackerCreature.Level / 5f + 2) * move.Power * (attackStat / (float)defenseStat)) / 50 + 2) * modifier);
        targetCreature.TakeDamage((int)MathF.Floor(damage));

        yield return context.Write($"{context.Defender.ActiveCreature.Data.Name} took {damage} damage!");
        yield return new WaitForSeconds(1.0f);
    }
}

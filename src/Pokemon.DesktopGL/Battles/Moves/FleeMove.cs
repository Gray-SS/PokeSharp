using System.Collections;
using PokeSharp.Engine.Coroutines;

namespace Pokemon.DesktopGL.Battles.Moves;

public class FleeMove : IBattleMove
{
    public IEnumerator Before(BattleContext context)
    {
        yield return null;
    }

    public IEnumerator After(BattleContext context)
    {
        yield return null;
    }

    public IEnumerator Execute(BattleContext context)
    {
        yield return context.Write($"You're fleeing from battle!");
        yield return new WaitForSeconds(1.0f);

        context.Battle.Flee();
    }
}
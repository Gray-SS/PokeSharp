using System.Collections;

namespace Pokemon.DesktopGL.Battles.Moves;

public class FleeMove : IBattleMove
{
    public string Name => "Fuite";

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
        yield return context.Write($"Vous prenez la fuite !");
        context.Battle.Flee();
    }
}
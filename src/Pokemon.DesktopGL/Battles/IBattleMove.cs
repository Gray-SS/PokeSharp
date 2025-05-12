using System.Collections;

namespace Pokemon.DesktopGL.Battles;

public interface IBattleMove
{
    public IEnumerator Before(BattleContext context);
    public IEnumerator Execute(BattleContext context);
    public IEnumerator After(BattleContext context);
}
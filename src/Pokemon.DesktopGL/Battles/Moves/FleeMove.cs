namespace Pokemon.DesktopGL.Battles.Moves;

public class FleeMove : IBattleMove
{
    public string Name => "Fuite";

    public void Execute(Combatant attacker, Combatant target)
    {
        attacker.Battle.Flee();
    }
}
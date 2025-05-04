namespace Pokemon.DesktopGL.Battles;

public interface IBattleMove
{
    public string Name { get; }

    public void Execute(Combatant attacker, Combatant target);
}
namespace Pokemon.DesktopGL.Battles.Moves;

public class AttackMove : IBattleMove
{
    public string Name => "Attaque";

    public void Execute(Combatant attacker, Combatant target)
    {
        int attack = attacker.ActiveCreature.Attack;
        target.ActiveCreature.TakeDamage(attack);
    }
}
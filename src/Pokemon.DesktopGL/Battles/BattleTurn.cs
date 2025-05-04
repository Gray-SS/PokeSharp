using System.Security;

namespace Pokemon.DesktopGL.Battles;

public readonly struct BattleTurn
{
    public Battle Battle { get; }

    public Combatant Player { get; }
    public Combatant Opponent { get; }

    public IBattleMove PlayerMove { get; }
    public IBattleMove OpponentMove { get; }

    public BattleTurn(Battle battle, Combatant player, Combatant opponent, IBattleMove playerMove, IBattleMove opponentMove)
    {
        Battle = battle;
        Player = player;
        Opponent = opponent;
        PlayerMove = playerMove;
        OpponentMove = opponentMove;
    }
}
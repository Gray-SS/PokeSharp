using System;
using Pokemon.DesktopGL.Battles.Moves;

namespace Pokemon.DesktopGL.Battles;

public sealed class Battle
{
    public Combatant Player { get; }
    public Combatant Opponent { get; }
    public BattleState State { get; private set; }

    public Combatant Winner { get; private set; }

    private IBattleMove _playerMove;
    private IBattleMove _opponentMove;

    public bool IsFinished { get; private set; }

    public Battle(Combatant player, Combatant opponent)
    {
        Player = player;
        Opponent = opponent;
        State = BattleState.Intro;

        Player.BindBattle(this);
        Opponent.BindBattle(this);
    }

    public void Start()
    {
        State = BattleState.WaitingForPlayerAction;
    }

    public void Flee()
    {
        State = BattleState.Fleed;
    }

    public BattleTurn GetTurn()
    {
        return new BattleTurn(
            this,
            Player,
            Opponent,
            _playerMove,
            _opponentMove
        );
    }

    public void StartNewTurn()
    {
        _playerMove = null;
        _opponentMove = null;
        State = BattleState.WaitingForPlayerAction;
    }

    public void EndBattle(Combatant winner)
    {
        Winner = winner;
        State = BattleState.BattleOver;
    }

    public void SelectMove(IBattleMove move)
    {
        if (State != BattleState.WaitingForPlayerAction)
            return;

        _playerMove = move;
        _opponentMove = DetermineOpponentMove();

        State = BattleState.PerformingTurn;
    }

    private FightMove DetermineOpponentMove()
    {
        var moveIndex = Random.Shared.Next(Opponent.ActiveCreature.Moves.Count);
        var move = Opponent.ActiveCreature.Moves[moveIndex];
        return new FightMove(move);
    }
}
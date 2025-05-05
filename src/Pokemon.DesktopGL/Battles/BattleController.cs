using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Pokemon.DesktopGL.Battles.Moves;
using Pokemon.DesktopGL.Core.Coroutines;
using Pokemon.DesktopGL.Core.Managers;
using Pokemon.DesktopGL.Core.Screens;
using Pokemon.DesktopGL.Miscellaneous;

namespace Pokemon.DesktopGL.Battles;

public sealed class BattleController
{
    public Battle Battle { get; }
    public TextTyper TextTyper { get; }

    private bool _isAnimating;
    private BattleCreatureRenderer _playerRenderer;
    private BattleCreatureRenderer _opponentRenderer;

    public BattleController(Battle battle, BattleCreatureRenderer playerRenderer, BattleCreatureRenderer opponentRenderer)
    {
        Battle = battle;
        TextTyper = new TextTyper(0.02f);

        _playerRenderer = playerRenderer;
        _opponentRenderer = opponentRenderer;

        CoroutineManager.Start(PlayIntro());
    }

    public void Update()
    {
        if (_isAnimating)
            return;

        switch (Battle.State)
        {
            case BattleState.PerformingTurn:
                CoroutineManager.Start(ExecuteTurn());
                break;
        }
    }

    private IEnumerator PlayOutro()
    {
        if (Battle.State == BattleState.BattleOver)
        {
            if (Battle.Winner == Battle.Player)
                yield return TextTyper.Write($"Vous avez gagné !");
            else
                yield return TextTyper.Write($"Vous avez perdu !");
        }
        else if (Battle.State == BattleState.Fleed)
            yield return TextTyper.Write("Vous avez prit la fuite.");

        yield return new WaitForSeconds(1.0f);

        var screenManager = PokemonGame.Instance.ScreenManager;
        var screen = screenManager.ActiveScreen;

        yield return screen.FadeIn();
        screenManager.Pop();
    }

    private IEnumerator PlayIntro()
    {
        _isAnimating = true;

        yield return TextTyper.Write($"Un {Battle.Opponent.ActiveCreature.Data.Name} sauvage apparait !");
        yield return new WaitForSeconds(1f);

        Battle.Start();

        yield return ShowActionPrompt();

        _isAnimating = false;
    }

    private IEnumerator ShowActionPrompt()
    {
        yield return TextTyper.Write($"Qu'est ce que {Battle.Player.ActiveCreature.Data.Name} doit faire ?");
    }

    private IEnumerator ExecuteTurn()
    {
        _isAnimating = true;

        BattleTurn turn = Battle.GetTurn();

        yield return ExecuteMove(turn.Player, turn.PlayerMove);

        if (Battle.State is BattleState.BattleOver or BattleState.Fleed)
        {
            yield return PlayOutro();
            yield break;
        }

        yield return new WaitForSeconds(1.0f);
        yield return ExecuteMove(turn.Opponent, turn.OpponentMove);

        if (Battle.State != BattleState.BattleOver)
        {
            Battle.StartNewTurn();
            yield return ShowActionPrompt();
        }
        else yield return PlayOutro();

        _isAnimating = false;
    }

    private IEnumerator ExecuteMove(Combatant combatant, IBattleMove move)
    {
        Combatant opponent = combatant == Battle.Player ? Battle.Opponent : Battle.Player;
        BattleCreatureRenderer renderer = combatant == Battle.Player ? _playerRenderer : _opponentRenderer;
        BattleCreatureRenderer opponentRenderer = combatant == Battle.Player ? _opponentRenderer : _playerRenderer;

        yield return TextTyper.Write($"{combatant.ActiveCreature.Data.Name} utilise {move.Name}");
        yield return new WaitForSeconds(1.0f);

        if (move is AttackMove)
        {
            CoroutineManager.Start(renderer.PlayAttackAnimation());
            CoroutineManager.Start(opponentRenderer.PlayTakeDamageAnimation());
        }

        move.Execute(combatant, opponent);

        if (opponent.ActiveCreature.IsFainted)
        {
            yield return opponentRenderer.PlayFaintAnimation();

            yield return TextTyper.Write($"{opponent.ActiveCreature.Data.Name} a été mit K.O");
            yield return new WaitForSeconds(1.0f);

            Battle.EndBattle(combatant);
        }
    }
}
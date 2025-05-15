using System.Collections;
using Pokemon.DesktopGL.Battles.Moves;
using Pokemon.DesktopGL.Miscellaneous;
using PokeSharp.Engine.Coroutines;
using PokeSharp.Engine.Managers;

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
                yield return TextTyper.Write($"You won!");
            else
                yield return TextTyper.Write($"You lost!");
        }
        else if (Battle.State == BattleState.Fleed)
            yield return TextTyper.Write("You fled from battle.");

        yield return new WaitForSeconds(1.0f);

        var screenManager = PokemonGame.Instance.ScreenManager;
        var screen = screenManager.ActiveScreen;

        yield return screen.FadeIn();
        screenManager.Pop();
    }

    private IEnumerator PlayIntro()
    {
        _isAnimating = true;

        yield return TextTyper.Write($"A wild {Battle.Opponent.ActiveCreature.Data.Name} appeared!");
        yield return new WaitForSeconds(1f);

        Battle.Start();

        yield return ShowActionPrompt();

        _isAnimating = false;
    }

    private IEnumerator ShowActionPrompt()
    {
        yield return TextTyper.Write($"What should {Battle.Player.ActiveCreature.Data.Name} do?");
    }

    private IEnumerator ExecuteTurn()
    {
        _isAnimating = true;

        BattleTurn turn = Battle.GetTurn();
        BattleContext context = new BattleContext(Battle, TextTyper.Write);

        context.Attacker = Battle.Player;
        context.AttackerMove = turn.PlayerMove;
        context.Defender = Battle.Opponent;
        yield return ExecuteMove(context);

        if (Battle.State is BattleState.BattleOver or BattleState.Fleed)
        {
            yield return PlayOutro();
            yield break;
        }

        context.Attacker = Battle.Opponent;
        context.AttackerMove = turn.OpponentMove;
        context.Defender = Battle.Player;
        yield return ExecuteMove(context);

        if (Battle.State != BattleState.BattleOver)
        {
            Battle.StartNewTurn();
            yield return ShowActionPrompt();
        }
        else yield return PlayOutro();

        _isAnimating = false;
    }

    private IEnumerator ExecuteMove(BattleContext context)
    {
        Combatant attacker = context.Attacker;
        IBattleMove attackerMove = context.AttackerMove;
        Combatant defender = context.Defender;

        BattleCreatureRenderer attackerRenderer = attacker.IsPlayer ? _playerRenderer : _opponentRenderer;
        BattleCreatureRenderer defenderRenderer = attacker.IsPlayer ? _opponentRenderer : _playerRenderer;

        yield return attackerMove.Before(context);

        if (attackerMove is FightMove)
        {
            CoroutineManager.Start(attackerRenderer.PlayAttackAnimation());
            CoroutineManager.Start(defenderRenderer.PlayTakeDamageAnimation());
        }

        yield return attackerMove.Execute(context);
        yield return attackerMove.After(context);

        if (defender.ActiveCreature.IsFainted)
        {
            yield return defenderRenderer.PlayFaintAnimation();

            yield return TextTyper.Write($"{defender.ActiveCreature.Data.Name} fainted!");
            yield return new WaitForSeconds(1.0f);

            if (attacker.IsPlayer)
            {
                int gainedEXP = attacker.ActiveCreature.GainEXP(defender.ActiveCreature);
                yield return TextTyper.Write($"You gained {gainedEXP} EXP from beating {defender.ActiveCreature.Data.Name}.");
                yield return new WaitForSeconds(1.0f);
            }

            Battle.EndBattle(attacker);
        }
    }
}
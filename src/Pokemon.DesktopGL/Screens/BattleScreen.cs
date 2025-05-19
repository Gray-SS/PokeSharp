using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Battles;
using Pokemon.DesktopGL.Battles.Moves;
using PokeSharp.Engine.Graphics;
using PokeSharp.Engine.Managers;
using PokeSharp.Engine.Renderers;
using Pokemon.DesktopGL.Creatures;
using Pokemon.DesktopGL.Moves;
using PokeSharp.Engine;
using PokeSharp.Engine.Assets;

namespace Pokemon.DesktopGL.Screens;

public enum BattleScreenState
{
    ActionPickup,
    AttackPickup
}

public sealed class BattleScreen : Screen
{
    public new PokemonGame Engine => GetEngine<PokemonGame>();

    private UIRenderer _uiRenderer;

    private int _selectedIndex;
    private int _maxSelectionIndex = 4;
    private SpriteFontBase _font;
    private BattleScreenState _state;

    private BattleCreatureRenderer _playerRenderer;
    private BattleCreatureRenderer _opponentRenderer;

    private Battle _battle;
    private BattleController _battleController;

    private Combatant _player;
    private Combatant _opponent;

    private Creature _opponentCreature;
    public BattleScreen(Creature opponent)
    {
        _opponentCreature = opponent;
    }

    protected override void Initialize()
    {
        _uiRenderer = new UIRenderer(GraphicsDevice);
        _font = Engine.AssetsManager.Font_PowerGreen.GetFont(30);

        _player = new Combatant(Engine.PlayerData.Creatures);
        _opponent = new Combatant([ _opponentCreature ]);

        _playerRenderer = new BattleCreatureRenderer(_player);
        _opponentRenderer = new BattleCreatureRenderer(_opponent);

        _battle = new Battle(_player, _opponent);
        _battleController = new BattleController(_battle, _playerRenderer, _opponentRenderer);

        ChangeState(BattleScreenState.ActionPickup);
    }

    protected override void Load()
    {
        CoroutineManager.Start(FadeOut());
    }

    protected override void Update(GameTime gameTime)
    {
        _battleController.Update();

        if (_battle.State != BattleState.WaitingForPlayerAction)
            return;

        var inputManager = Engine.InputManager;

        if (inputManager.IsKeyPressed(Keys.D) || inputManager.IsKeyPressed(Keys.Right))
            _selectedIndex = (_selectedIndex + 1) % _maxSelectionIndex;

        if (inputManager.IsKeyPressed(Keys.A) || inputManager.IsKeyPressed(Keys.Left))
            _selectedIndex = (_selectedIndex + _maxSelectionIndex - 1) % _maxSelectionIndex;

        if (inputManager.IsKeyPressed(Keys.W) || inputManager.IsKeyPressed(Keys.Up))
            _selectedIndex = (_selectedIndex + _maxSelectionIndex - 2) % _maxSelectionIndex;

        if (inputManager.IsKeyPressed(Keys.S) || inputManager.IsKeyPressed(Keys.Down))
            _selectedIndex = (_selectedIndex + 2) % _maxSelectionIndex;

        if (inputManager.IsKeyPressed(Keys.Enter))
        {
            if (_state == BattleScreenState.ActionPickup)
            {
                switch (_selectedIndex)
                {
                    case 0:
                        ChangeState(BattleScreenState.AttackPickup);
                        break;
                    case 3:
                        _battle.SelectMove(new FleeMove());
                        break;
                };
            }
            else if (_state == BattleScreenState.AttackPickup)
            {
                var moves = _player.ActiveCreature.Moves;
                if (_selectedIndex < 0 || _selectedIndex >= moves.Count)
                    return;

                MoveData move = moves[_selectedIndex];
                _battle.SelectMove(new FightMove(move));
                ChangeState(BattleScreenState.ActionPickup);
            }
        }
    }

    private void ChangeState(BattleScreenState state)
    {
        _state = state;

        switch (state)
        {
            case BattleScreenState.ActionPickup:
                _selectedIndex = 0;
                _maxSelectionIndex = 4;
                break;
            case BattleScreenState.AttackPickup:
                _selectedIndex = 0;
                _maxSelectionIndex = _player.ActiveCreature.Moves.Count;
                break;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        _uiRenderer.Begin();

        Rectangle bounds = Engine.WindowManager.Rect;
        DrawBackground(bounds);

        Rectangle gameBounds = bounds;
        gameBounds.Height = (int)Engine.WindowManager.AlignY(0.70f);

        DrawBaseSelf(gameBounds);
        DrawBaseOther(gameBounds);

        DrawDataboxSelf(gameBounds);
        DrawDataboxOther(gameBounds);

        Rectangle actionBounds = bounds;
        actionBounds.Height = (int)Engine.WindowManager.AlignY(0.3f);
        actionBounds.Y = (int)Engine.WindowManager.AlignY(0.70f);

        DrawBottomContainer(actionBounds);

        _uiRenderer.End();
    }

    private void DrawBackground(Rectangle bounds)
    {
        _uiRenderer.Draw(Engine.AssetsManager.Sprite_Battle_Forest_Bg, bounds, Color.White);
    }

    private void DrawBaseSelf(Rectangle bounds)
    {
        Sprite sprite = Engine.AssetsManager.Sprite_Battle_Grass_Base0;

        int width = (int)(bounds.Width * 0.7f);
        int height = (int)(width * 0.125f);
        int x = -100;
        int y = bounds.Height - height;
        bounds = new Rectangle(x, y, width, height);
        _uiRenderer.Draw(sprite, bounds, Color.White);

        _playerRenderer.Draw(_uiRenderer, bounds);
    }

    private void DrawBaseOther(Rectangle bounds)
    {
        Sprite sprite = Engine.AssetsManager.Sprite_Battle_Grass_Base1;

        int width = (int)(bounds.Width * 0.45f);
        int height = (int)(width * 0.5f);
        int x = bounds.X + bounds.Width - width;
        int y = bounds.Y + (bounds.Height - height) / 2;
        bounds = new Rectangle(x, y, width, height);
        _uiRenderer.Draw(sprite, bounds, Color.White);

        _opponentRenderer.Draw(_uiRenderer, bounds);
    }

    private void DrawDataboxSelf(Rectangle bounds)
    {
        Sprite sprite = Engine.AssetsManager.Sprite_Battle_Databox;

        int width = (int)(bounds.Width * 0.352f);
        int height = (int)(width * 0.32f);
        int x = bounds.Width - width;
        int y = (int)(bounds.Height * 0.9f - height);
        bounds = new Rectangle(x, y, width, height);
        _uiRenderer.Draw(sprite, bounds, Color.White);

        var font = Engine.AssetsManager.Font_PowerGreen.GetFont(35);
        var creature = _player.ActiveCreature;
        _uiRenderer.DrawString(font, creature.Data.Name, new Vector2(x + 75, y + 10), Color.Black);

        var hpScale = creature.HP / (float)creature.MaxHP;
        var hpOverlayIndex = hpScale switch
        {
            >= 0.6f => 0,
            >= 0.3f => 1,
            _ => 2
        };

        var hpOverlayBounds = bounds;
        hpOverlayBounds.X = bounds.Center.X + 10;
        hpOverlayBounds.Width = (int)((bounds.Width / 2 - 55) * hpScale);
        hpOverlayBounds.Y = bounds.Center.Y - 3;
        hpOverlayBounds.Height = 10;
        _uiRenderer.Draw(Engine.AssetsManager.Sheet_Battle_Hp_Overlay.GetSprite(hpOverlayIndex), hpOverlayBounds, Color.White);

        var font25 = Engine.AssetsManager.Font_PowerGreen.GetFont(25);
        var hpPosition = new Vector2(bounds.Center.X + 35, bounds.Center.Y + 10);
        _uiRenderer.DrawString(font25, $"{creature.HP}/{creature.MaxHP}", hpPosition, Color.Black);

        var fontBold = Engine.AssetsManager.Font_PowerClearBold.GetFont(22);
        var lvlPosition = new Vector2(bounds.Right - 115, bounds.Top + 25);
        _uiRenderer.DrawString(fontBold, $"Lv.{creature.Level}", lvlPosition, Color.Black);
    }

    private void DrawDataboxOther(Rectangle bounds)
    {
        var sprite = Engine.AssetsManager.Sprite_Battle_Databox_Foe;

        int width = (int)(bounds.Width * 0.352f);
        int height = (int)(width * 0.22f);
        int x = 0;
        int y = (int)(bounds.Height * 0.15f);
        bounds = new Rectangle(x, y, width, height);
        _uiRenderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);

        var font = Engine.AssetsManager.Font_PowerGreen.GetFont(35);
        var creature = _opponent.ActiveCreature;
        _uiRenderer.DrawString(font, creature.Data.Name, new Vector2(x + 10, y + 6), Color.Black);

        var hpScale = creature.HP / (float)creature.MaxHP;
        var hpOverlayBounds = bounds;
        var hpOverlayIndex = hpScale switch
        {
            >= 0.6f => 0,
            >= 0.3f => 1,
            _ => 2
        };

        hpOverlayBounds.X = bounds.Center.X - 21;
        hpOverlayBounds.Width = (int)((bounds.Width / 2 - 55) * hpScale);
        hpOverlayBounds.Y = bounds.Center.Y + 15;
        hpOverlayBounds.Height = 10;
        _uiRenderer.Draw(Engine.AssetsManager.Sheet_Battle_Hp_Overlay.GetSprite(hpOverlayIndex), hpOverlayBounds, Color.White);

        var fontBold = Engine.AssetsManager.Font_PowerClearBold.GetFont(22);
        var lvlPosition = new Vector2(bounds.Right - 145, bounds.Top + 20);
        _uiRenderer.DrawString(fontBold, $"Lv.{creature.Level}", lvlPosition, Color.Black);
    }

    private void DrawBottomContainer(Rectangle bounds)
    {
        var assetsManager = Engine.AssetsManager;
        _uiRenderer.Draw(assetsManager.Sprite_Blank, new Rectangle(bounds.Left, bounds.Top, bounds.Width, 10), Color.Black);

        bounds.Y += 5;
        bounds.Height -= 10;

        if (_state == BattleScreenState.ActionPickup)
            DrawActionPickup(bounds);
        else if (_state == BattleScreenState.AttackPickup)
            DrawAttackPickup(bounds);
    }

    private void DrawActionPickup(Rectangle bounds)
    {
        var leftContainerBounds = bounds;
        leftContainerBounds.Width /= 2;

        DrawOverlayMessage(leftContainerBounds);

        var rightContainerBounds = leftContainerBounds;
        rightContainerBounds.X += leftContainerBounds.Width + 50;
        rightContainerBounds.Width -= 100;

        DrawActions(rightContainerBounds);
    }

    private void DrawOverlayMessage(Rectangle bounds)
    {
        var assetsManager = Engine.AssetsManager;
        bounds.X += 40;
        bounds.Width -= 40;
        _uiRenderer.Draw(assetsManager.Sprite_Dialogue_Overlay, bounds, Color.White);

        var text = _battleController.TextTyper.CurrentText.ToString();
        var size = _font.MeasureString(text);
        var position = new Vector2(bounds.X + 50, bounds.Y + bounds.Height * 0.5f - size.Y * 0.5f);
        _uiRenderer.DrawWrappedText(_font, text, position, Color.Black, bounds.Width - 100);
    }

    private void DrawActions(Rectangle bounds)
    {
        bounds.X += 10;
        bounds.Y += 10;
        bounds.Width -= 20;
        bounds.Height -= 20;

        int btnWidth = bounds.Width / 2;
        int btnHeight = bounds.Height / 2;

        var sheet = Engine.AssetsManager.Sheet_Battle_Cursor_Action;

        for (int i = 0; i < 4; i++)
        {
            int col = i % 2;
            int row = i / 2;

            int x = bounds.X + col * btnWidth;
            int y = bounds.Y + row * btnHeight;
            var btnBounds = new Rectangle(x, y, btnWidth, btnHeight);

            var index = i;
            if (index % 2 != 0)
                index += 3;

            if (_selectedIndex == i)
                index++;

            _uiRenderer.Draw(sheet.GetSprite(index), btnBounds, Color.White);
        }
    }

    private void DrawAttackPickup(Rectangle bounds)
    {
        bounds.X += 10;
        bounds.Y += 10;
        bounds.Width -= 20;
        bounds.Height -= 20;

        var assetsManager = PokemonGame.Instance.AssetsManager;
        var bg = assetsManager.Sprite_Battle_Overlay_Fight;
        _uiRenderer.Draw(bg, bounds, Color.White);

        DrawAttackSelection(assetsManager, bounds);
        DrawAttackInfo(assetsManager, bounds);
    }

    private void DrawAttackInfo(AssetsManager assetsManager, Rectangle bounds)
    {
        bounds.X = (int)(bounds.Width * 0.765f) + 45;
        bounds.Y += 50;
        bounds.Width = (int)(bounds.Width * 0.235f) - 90;
        bounds.Height -= 90;

        MoveData selectedMove = _player.ActiveCreature.Moves[_selectedIndex];

        var font = assetsManager.Font_PowerClearBold.GetFont(20);
        var color = new Color(60, 60, 60);

        // TODO: Change this to use the base PP
        string movePPText = $"{selectedMove.PP}/{selectedMove.PP}";
        Vector2 movePPSize = font.MeasureString(movePPText);
        Vector2 movePPPos = new Vector2(bounds.Right, bounds.Top) - movePPSize * new Vector2(1.0f, 0.0f);
        Vector2 ppPos = new Vector2(bounds.Left, bounds.Top);

        _uiRenderer.DrawString(font, "PP", ppPos, color);
        _uiRenderer.DrawString(font, movePPText, movePPPos, color);

        string moveTypeText = $"TYPE/{selectedMove.Type.ToString().ToUpper()}";
        Vector2 moveTypeSize = font.MeasureString(movePPText);
        Vector2 moveTypePos = new Vector2(bounds.Left, bounds.Bottom) - moveTypeSize * new Vector2(0.0f, 1.0f);
        _uiRenderer.DrawString(font, moveTypeText, moveTypePos, color);
    }

    private void DrawAttackSelection(AssetsManager assetsManager, Rectangle bounds)
    {
        var sheet = assetsManager.Sheet_Battle_Cursor_Fight;

        var moves = _player.ActiveCreature.Moves;

        int startX = bounds.X + 11;
        int startY = bounds.Y + 11;
        int width = (bounds.Width - 330) / 2;
        int height = (bounds.Height - 16) / 2;

        for (int i = 0; i < moves.Count; i++)
        {
            MoveData move = moves[i];

            int data = (int)move.Type * 2;
            bool isSelected = _selectedIndex == i;
            if (isSelected) data++;

            int col = i % 2;
            int row = i / 2;

            int x = startX + width * col;
            int y = startY + height * row;
            var attackBounds = new Rectangle(x, y, width, height);

            _uiRenderer.Draw(sheet.GetSprite(data), attackBounds, Color.White);

            Color color = isSelected ? new Color(90, 90, 90) : new Color(60, 60, 60);
            var font = assetsManager.Font_PowerClearBold.GetFont(25);
            Vector2 moveNameSize = font.MeasureString(move.Name);
            Vector2 moveNamePos = attackBounds.Center.ToVector2() - moveNameSize * 0.5f;
            _uiRenderer.DrawString(font, move.Name, moveNamePos, color);
        }
    }
}
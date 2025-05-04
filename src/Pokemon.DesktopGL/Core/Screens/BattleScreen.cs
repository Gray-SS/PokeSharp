using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Battles;
using Pokemon.DesktopGL.Battles.Moves;
using Pokemon.DesktopGL.Core.Graphics;
using Pokemon.DesktopGL.Core.Renderers;
using Pokemon.DesktopGL.Creatures;

namespace Pokemon.DesktopGL.Core.Screens;

public sealed class BattleScreen : Screen
{
    private UIRenderer _uiRenderer;

    private int _selectedIndex;
    private SpriteFontBase _font;

    private BattleCreatureRenderer _playerRenderer;
    private BattleCreatureRenderer _opponentRenderer;

    private Battle _battle;
    private BattleController _battleController;

    private Combatant _player;
    private Combatant _opponent;

    private CreatureData _opponentData;

    public BattleScreen(CreatureData opponentData)
    {
        _opponentData = opponentData;
    }

    public override void Load()
    {
        _uiRenderer = new UIRenderer(GraphicsDevice);
        _font = Game.AssetsManager.Font_PowerGreen.GetFont(30);

        _player = new Combatant(Game.PlayerData.Creatures);
        _opponent = new Combatant([ _opponentData.Create(4) ]);

        _playerRenderer = new BattleCreatureRenderer(_player);
        _opponentRenderer = new BattleCreatureRenderer(_opponent);

        _battle = new Battle(_player, _opponent);
        _battleController = new BattleController(_battle, _playerRenderer, _opponentRenderer);
    }

    public override void Update(GameTime gameTime)
    {
        _battleController.Update();

        if (_battle.State != BattleState.WaitingForPlayerAction)
            return;

        var inputManager = Game.InputManager;

        if (inputManager.IsKeyPressed(Keys.D) || inputManager.IsKeyPressed(Keys.Right))
            _selectedIndex = (_selectedIndex + 1) % 4;

        if (inputManager.IsKeyPressed(Keys.A) || inputManager.IsKeyPressed(Keys.Left))
            _selectedIndex = (_selectedIndex + 4 - 1) % 4;

        if (inputManager.IsKeyPressed(Keys.W) || inputManager.IsKeyPressed(Keys.Up))
            _selectedIndex = (_selectedIndex + 4 - 2) % 4;

        if (inputManager.IsKeyPressed(Keys.S) || inputManager.IsKeyPressed(Keys.Down))
            _selectedIndex = (_selectedIndex + 2) % 4;

        if (inputManager.IsKeyPressed(Keys.Enter))
        {
            IBattleMove move = _selectedIndex switch
            {
                0 => new AttackMove(),
                1 => null,
                2 => null,
                3 => new FleeMove(),
                _ => null
            };

            if (move == null)
                return;

            _battle.SelectMove(move);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        _uiRenderer.Begin();

        Rectangle bounds = Game.WindowManager.Rect;
        DrawBackground(bounds);

        Rectangle gameBounds = bounds;
        gameBounds.Height = (int)Game.WindowManager.AlignY(0.70f);

        DrawBaseSelf(gameBounds);
        DrawBaseOther(gameBounds);

        DrawDataboxSelf(gameBounds);
        DrawDataboxOther(gameBounds);

        Rectangle actionBounds = bounds;
        actionBounds.Height = (int)Game.WindowManager.AlignY(0.3f);
        actionBounds.Y = (int)Game.WindowManager.AlignY(0.70f);

        DrawActionContainer(actionBounds);

        _uiRenderer.End();
    }

    private void DrawBackground(Rectangle bounds)
    {
        _uiRenderer.Draw(Game.AssetsManager.Sprite_Battle_Forest_Bg, bounds, Color.White);
    }

    private void DrawBaseSelf(Rectangle bounds)
    {
        Sprite sprite = Game.AssetsManager.Sprite_Battle_Grass_Base0;

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
        Sprite sprite = Game.AssetsManager.Sprite_Battle_Grass_Base1;

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
        Sprite sprite = Game.AssetsManager.Sprite_Battle_Databox;

        int width = (int)(bounds.Width * 0.352f);
        int height = (int)(width * 0.32f);
        int x = bounds.Width - width;
        int y = (int)(bounds.Height * 0.9f - height);
        bounds = new Rectangle(x, y, width, height);
        _uiRenderer.Draw(sprite, bounds, Color.White);

        var font = Game.AssetsManager.Font_PowerGreen.GetFont(35);
        var creature = _player.ActiveCreature;
        _uiRenderer.DrawString(font, creature.Data.Name, new Vector2(x + 75, y + 10), Color.Black);

        var hpScale = creature.HP / (float)creature.MaxHP;
        var hpOverlayBounds = bounds;
        hpOverlayBounds.X = bounds.Center.X + 10;
        hpOverlayBounds.Width = (int)((bounds.Width / 2 - 55) * hpScale);
        hpOverlayBounds.Y = bounds.Center.Y - 3;
        hpOverlayBounds.Height = 10;
        _uiRenderer.Draw(Game.AssetsManager.Sheet_Battle_Hp_Overlay.GetSprite(0), hpOverlayBounds, Color.White);

        font = Game.AssetsManager.Font_PowerGreen.GetFont(25);
        var hpPosition = new Vector2(bounds.Center.X + 35, bounds.Center.Y + 10);
        _uiRenderer.DrawString(font, $"{creature.HP}/{creature.MaxHP}", hpPosition, Color.Black);
    }

    private void DrawDataboxOther(Rectangle bounds)
    {
        var sprite = Game.AssetsManager.Sprite_Battle_Databox_Foe;

        int width = (int)(bounds.Width * 0.352f);
        int height = (int)(width * 0.22f);
        int x = 0;
        int y = (int)(bounds.Height * 0.15f);
        bounds = new Rectangle(x, y, width, height);
        _uiRenderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);

        var font = Game.AssetsManager.Font_PowerGreen.GetFont(35);
        var creature = _opponent.ActiveCreature;
        _uiRenderer.DrawString(font, creature.Data.Name, new Vector2(x + 10, y + 6), Color.Black);

        var hpScale = creature.HP / (float)creature.MaxHP;
        var hpOverlayBounds = bounds;
        hpOverlayBounds.X = bounds.Center.X - 21;
        hpOverlayBounds.Width = (int)((bounds.Width / 2 - 55) * hpScale);
        hpOverlayBounds.Y = bounds.Center.Y + 15;
        hpOverlayBounds.Height = 10;
        _uiRenderer.Draw(Game.AssetsManager.Sheet_Battle_Hp_Overlay.GetSprite(0), hpOverlayBounds, Color.White);
    }

    private void DrawActionContainer(Rectangle bounds)
    {
        var assetsManager = Game.AssetsManager;
        _uiRenderer.Draw(assetsManager.Sprite_Blank, new Rectangle(bounds.Left, bounds.Top, bounds.Width, 10), Color.Black);

        bounds.Y += 5;
        bounds.Height -= 10;

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
        var assetsManager = Game.AssetsManager;
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

        var sheet = Game.AssetsManager.Sheet_Battle_Actions;

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
}
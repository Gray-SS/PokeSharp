using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Core.Renderers;

namespace Pokemon.DesktopGL.Core.Screens;

public sealed class BattleScreen : Screen
{
    private UIRenderer _uiRenderer;

    private int _selectedIndex;
    private SpriteFontBase _font;

    public BattleScreen(PokemonGame game) : base(game)
    {
    }

    public override void Load()
    {
        _uiRenderer = new UIRenderer(GraphicsDevice);
        _font = Game.AssetsManager.Font_PowerGreen.GetFont(30);
    }

    public override void Update(GameTime gameTime)
    {
        var inputManager = Game.InputManager;

        if (inputManager.IsKeyPressed(Keys.D) || inputManager.IsKeyPressed(Keys.Right))
            _selectedIndex = (_selectedIndex + 1) % 4;

        if (inputManager.IsKeyPressed(Keys.A) || inputManager.IsKeyPressed(Keys.Left))
            _selectedIndex = (_selectedIndex + 4 - 1) % 4;

        if (inputManager.IsKeyPressed(Keys.W) || inputManager.IsKeyPressed(Keys.Up))
            _selectedIndex = (_selectedIndex + 4 - 2) % 4;

        if (inputManager.IsKeyPressed(Keys.S) || inputManager.IsKeyPressed(Keys.Down))
            _selectedIndex = (_selectedIndex + 2) % 4;
    }

    public override void Draw(GameTime gameTime)
    {
        _uiRenderer.Begin();

        Rectangle bounds = Game.WindowManager.Rect;
        DrawBackground(bounds);

        Rectangle gameBounds = bounds;
        gameBounds.Height = (int)Game.WindowManager.AlignY(0.75f);

        DrawBaseSelf(gameBounds);
        DrawBaseOther(gameBounds);

        DrawDataboxSelf(gameBounds);
        DrawDataboxOther(gameBounds);

        Rectangle actionBounds = bounds;
        actionBounds.Height = (int)Game.WindowManager.AlignY(0.25f);
        actionBounds.Y = (int)Game.WindowManager.AlignY(0.75f);

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
        _uiRenderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);
    }

    private void DrawBaseOther(Rectangle bounds)
    {
        Sprite sprite = Game.AssetsManager.Sprite_Battle_Grass_Base1;

        int width = (int)(bounds.Width * 0.45f);
        int height = (int)(width * 0.5f);
        int x = bounds.X + bounds.Width - width;
        int y = bounds.Y + (bounds.Height - height) / 2;
        _uiRenderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);
    }

    private void DrawDataboxSelf(Rectangle bounds)
    {
        Sprite sprite = Game.AssetsManager.Sprite_Battle_Databox;

        int width = (int)(bounds.Width * 0.352f);
        int height = (int)(width * 0.32f);
        int x = bounds.Width - width;
        int y = (int)(bounds.Height * 0.9f - height);
        _uiRenderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);
    }

    private void DrawDataboxOther(Rectangle bounds)
    {
        var sprite = Game.AssetsManager.Sprite_Battle_Databox_Foe;

        int width = (int)(bounds.Width * 0.352f);
        int height = (int)(width * 0.22f);
        int x = 0;
        int y = (int)(bounds.Height * 0.15f);
        _uiRenderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);
    }

    private void DrawActionContainer(Rectangle bounds)
    {
        var assetsManager = Game.AssetsManager;
        _uiRenderer.Draw(assetsManager.Sprite_Blank, bounds, Color.White);

        var leftContainerBounds = bounds;
        leftContainerBounds.Width /= 2;

        DrawPlaceholderText(leftContainerBounds);

        var rightContainerBounds = leftContainerBounds;
        rightContainerBounds.X += leftContainerBounds.Width + 50;
        rightContainerBounds.Width -= 100;

        DrawActions(rightContainerBounds);
    }

    private void DrawPlaceholderText(Rectangle bounds)
    {
        var text = "Que souhaitez-vous faire ?";
        var size = _font.MeasureString(text);
        var position = new Vector2(bounds.X + 30, bounds.Y + bounds.Height * 0.5f - size.Y * 0.5f);
        _uiRenderer.DrawString(_font, text, position, Color.Black);
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
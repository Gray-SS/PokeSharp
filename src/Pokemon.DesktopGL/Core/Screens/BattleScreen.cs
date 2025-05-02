using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core.Managers;
using Pokemon.DesktopGL.Core.Renderers;

namespace Pokemon.DesktopGL.Core.Screens;

public sealed class BattleScreen : Screen
{
    private UIRenderer _uiRenderer;

    public BattleScreen(PokemonGame game) : base(game)
    {
    }

    public override void Load()
    {
        _uiRenderer = new UIRenderer(GraphicsDevice);
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime)
    {
        _uiRenderer.Begin();

        DrawBackground();
        DrawBaseSelf();
        DrawBaseOther();

        DrawDataboxSelf();
        DrawDataboxOther();

        _uiRenderer.End();
    }

    private void DrawBackground()
    {
        _uiRenderer.Draw(Game.AssetsManager.Sprite_Battle_Forest_Bg, Game.WindowManager.Bounds, Color.White);
    }

    private void DrawBaseSelf()
    {
        WindowManager winMan = Game.WindowManager;
        Sprite sprite = Game.AssetsManager.Sprite_Battle_Grass_Base0;

        int width = (int)(winMan.WindowWidth * 0.7f);
        int height = (int)(width * 0.125f);
        int x = -100;
        int y = winMan.WindowHeight - height;
        _uiRenderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);
    }

    private void DrawBaseOther()
    {
        WindowManager winMan = Game.WindowManager;
        Sprite sprite = Game.AssetsManager.Sprite_Battle_Grass_Base1;

        int width = (int)(winMan.WindowWidth * 0.45f);
        int height = (int)(width * 0.5f);
        int x = (int)(winMan.AlignX(1.0f) - width);
        int y = (int)(winMan.AlignY(0.5f) - height * 0.5f);
        _uiRenderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);
    }

    private void DrawDataboxSelf()
    {
        WindowManager winMan = Game.WindowManager;
        Sprite sprite = Game.AssetsManager.Sprite_Battle_Databox;

        int width = (int)(winMan.WindowWidth * 0.352f);
        int height = (int)(width * 0.32f);
        int x = (int)(winMan.AlignX(1.0f) - width);
        int y = (int)(winMan.AlignY(0.9f) - height);
        _uiRenderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);
    }

    private void DrawDataboxOther()
    {
        WindowManager winMan = Game.WindowManager;
        var sprite = Game.AssetsManager.Sprite_Battle_Databox_Foe;

        int width = (int)(winMan.WindowWidth * 0.352f);
        int height = (int)(width * 0.22f);
        int x = 0;
        int y = (int)winMan.AlignY(0.15f);
        _uiRenderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);
    }
}
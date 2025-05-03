using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core.Renderers;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.World;

namespace Pokemon.DesktopGL.Core.Screens;

public sealed class OverworldScreen : Screen
{
    private Camera _camera;
    private GameRenderer _gameRenderer;

    private GameMap _map;
    private Overworld _world;

    public override void Load()
    {
        _camera = new Camera(Game.WindowManager);
        _gameRenderer = new GameRenderer(GraphicsDevice);

        _map = GameMap.Load("Content/Data/Maps/test_map.tmx");

        _world = new Overworld(_map);
        _world.LoadEntities();

        Game.ActiveWorld = _world;
    }

    public override void Unload()
    {
        _world = null;
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _world.Update(dt);

        var target = _world.Player.Character.Position;

        var viewportSize = _camera.Viewport.Bounds.Size.ToVector2();
        var halfView = viewportSize * 0.5f;

        int mapPixelWidth  = (int)(_map.TiledMap.Width * GameConstants.TileSize);
        int mapPixelHeight = (int)(_map.TiledMap.Height * GameConstants.TileSize);

        float clampedX = MathHelper.Clamp(target.X, halfView.X + 20, mapPixelWidth  - halfView.X - 40);
        float clampedY = MathHelper.Clamp(target.Y, halfView.Y + 20, mapPixelHeight - halfView.Y - 40);

        Vector2 clampedTarget = new(clampedX, clampedY);
        _camera.Position = Vector2.Lerp(_camera.Position, clampedTarget, 0.05f);
    }


    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _gameRenderer.Begin(_camera);
        _world.Draw(_gameRenderer);

        _gameRenderer.End();
    }
}
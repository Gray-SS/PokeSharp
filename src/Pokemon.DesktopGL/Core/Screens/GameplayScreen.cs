using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core.Entities;
using Pokemon.DesktopGL.Core.Renderers;

namespace Pokemon.DesktopGL.Core.Screens;

public sealed class GameplayScreen : Screen
{
    private int _lastCol, _lastRow;
    private Camera _camera;
    private Tilemap _tilemap;
    private GameRenderer _gameRenderer;
    private TilemapEntity _tilemapEntity;

    public GameplayScreen(PokemonGame game) : base(game)
    {
    }

    public override void Load()
    {
        _camera = new Camera(Game.WindowManager);
        _gameRenderer = new GameRenderer(GraphicsDevice);

        var data = new int[30*30];
        for (int i = 0; i < 30*30; i++)
        {
            int col = i % 30;
            int row = i / 30;

            data[i] = 1 + (col + row) % 5;
        }

        SpawnBush(data, 10, 10, 6, 4);

        _tilemap = new Tilemap(30, 30, Game.AssetsManager.SpriteSheet_TS_Outside.Sprites, data);
        _tilemapEntity = new TilemapEntity(_tilemap, GameConstants.TileSize);
    }

    private void SpawnBush(int[] data, int x, int y, int width, int height)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                int k = j * 30 + i;
                data[k] = 6;
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Game.Player.Update(dt);
        _camera.Position = Vector2.Lerp(_camera.Position, Game.Player.Position, 0.05f);

        (int col, int row) = _tilemapEntity.GetCoord(Game.Player.Position);

        if (_lastCol != col || _lastRow != row)
        {
            int data = _tilemapEntity.GetData(col, row);
            if (data == 6)
            {

            }

            _lastCol = col;
            _lastRow = row;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _gameRenderer.Begin(_camera);

        _tilemapEntity.Draw(_gameRenderer);
        Game.Player.Draw(_gameRenderer);

        _gameRenderer.End();
    }
}
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.World;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core.Renderers;
using Pokemon.DesktopGL.NPCs;
using Pokemon.DesktopGL.Players;

namespace Pokemon.DesktopGL.Core.Screens;

public sealed class OverworldScreen : Screen
{
    private int _lastCol, _lastRow;
    private Camera _camera;
    private MapData _map;
    private GameRenderer _gameRenderer;
    private MapRenderer _mapRenderer;

    private CharacterSpawner _characterSpawner;

    private NPC _npc;
    private Player _player;

    public OverworldScreen(PokemonGame game) : base(game)
    {
    }

    public override void Load()
    {
        _camera = new Camera(Game.WindowManager);
        _gameRenderer = new GameRenderer(GraphicsDevice);

        _characterSpawner = new CharacterSpawner(Game.CharacterRegistry);
        _npc = _characterSpawner.SpawnNPC("npc_12", Vector2.Zero);
        _player = _characterSpawner.SpawnPlayer("player_girl", Vector2.Zero);

        var data = new int[30*30];
        for (int i = 0; i < 30*30; i++)
        {
            int col = i % 30;
            int row = i / 30;

            data[i] = 1 + (col + row) % 5;
        }

        SpawnBush(data, 10, 10, 6, 4);

        _map = new MapData(30, 30, Game.AssetsManager.Sheet_Tileset_Outside.Sprites, data);
        _mapRenderer = new MapRenderer(_map, GameConstants.TileSize);
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

        // _npc.Update(dt);
        _player.Update(dt);

        _camera.Position = Vector2.Lerp(_camera.Position, _player.Character.Position, 0.05f);

        (int col, int row) = _mapRenderer.GetCoord(_player.Character.Position);

        if (_lastCol != col || _lastRow != row)
        {
            int data = _mapRenderer.GetData(col, row);
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

        _mapRenderer.Draw(_gameRenderer);
        // _npc.Draw(_gameRenderer);
        _player.Draw(_gameRenderer);

        _gameRenderer.End();
    }
}
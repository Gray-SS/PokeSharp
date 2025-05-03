using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core.Renderers;
using Pokemon.DesktopGL.NPCs;
using Pokemon.DesktopGL.Players;
using Microsoft.Xna.Framework.Graphics;
using DotTiled.Serialization;
using Pokemon.DesktopGL.World;

namespace Pokemon.DesktopGL.Core.Screens;

public sealed class OverworldScreen : Screen
{
    private int _lastCol, _lastRow;
    private Camera _camera;
    private GameRenderer _gameRenderer;

    private DotTiled.Map _map;
    private TiledMapRenderer _mapRenderer;

    private CharacterSpawner _characterSpawner;

    private NPC _npc;
    private Player _player;

    public override void Load()
    {
        _camera = new Camera(Game.WindowManager);
        _gameRenderer = new GameRenderer(GraphicsDevice);

        _characterSpawner = new CharacterSpawner(Game.CharacterRegistry);
        _npc = _characterSpawner.SpawnNPC("npc_12", Vector2.Zero, new NPCData { Name = "Jean", Dialogues = []});
        _player = _characterSpawner.SpawnPlayer("player_boy", Vector2.Zero);

        var loader = Loader.Default();
        _map = loader.LoadMap("Content/Data/Maps/test_map.tmx");
        _mapRenderer = new TiledMapRenderer(_map);
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _npc.Update(dt);
        _player.Update(dt);

        _camera.Position = Vector2.Lerp(_camera.Position, _player.Character.Position, 0.05f);

        // (int col, int row) = _mapRenderer.GetCoord(_player.Character.Position);

        // if (_lastCol != col || _lastRow != row)
        // {

        //     _lastCol = col;
        //     _lastRow = row;
        // }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _gameRenderer.Begin(_camera);

        _mapRenderer.Draw(_gameRenderer);
        _npc.Draw(_gameRenderer);
        _player.Draw(_gameRenderer);

        _gameRenderer.End();
    }
}
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core.Renderers;
using Microsoft.Xna.Framework.Graphics;
using DotTiled.Serialization;
using Pokemon.DesktopGL.World;
using System.Linq;
using System.Collections.Generic;
using System;
using Pokemon.DesktopGL.Players;

namespace Pokemon.DesktopGL.Core.Screens;

public sealed class OverworldScreen : Screen
{
    private Camera _camera;
    private GameRenderer _gameRenderer;

    private GameMap _map;
    private TiledMapRenderer _mapRenderer;

    private Player _player;
    private EntitySpawner _spawner;
    private List<WorldEntity> _entities;

    public override void Load()
    {
        _camera = new Camera(Game.WindowManager);
        _gameRenderer = new GameRenderer(GraphicsDevice);

        _entities = new List<WorldEntity>();
        _spawner = new EntitySpawner(Game.CharacterRegistry);

        _map = GameMap.Load("Content/Data/Maps/test_map.tmx");
        _mapRenderer = new TiledMapRenderer(_map);

        foreach (DotTiled.ObjectLayer objectLayer in _map.TiledMap.Layers.OfType<DotTiled.ObjectLayer>())
        {
            foreach (DotTiled.Object obj in objectLayer.Objects)
            {
                if (obj.Type == "entity")
                {
                    var character_id = obj.GetProperty<DotTiled.StringProperty>("character_id").Value;
                    var dialogues = obj.GetProperty<DotTiled.StringProperty>("dialogues").Value;
                    var name = obj.GetProperty<DotTiled.StringProperty>("name").Value;
                    var type = obj.GetProperty<DotTiled.StringProperty>("type").Value;

                    var col = (int)(obj.X / _map.TiledMap.TileWidth);
                    var row = (int)(obj.Y / _map.TiledMap.TileHeight);

                    var entityDef = new EntityDefinition
                    {
                        Name = name,
                        Dialogues = [.. dialogues.Split(';')],
                        SpawnCol = col,
                        SpawnRow = row,
                        Type = Enum.Parse<EntityType>(type, true),
                        CharacterId = character_id
                    };

                    var entity = _spawner.Spawn(entityDef);
                    if (entity is Player player)
                        _player = player;

                    _entities.Add(entity);
                }
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entity in _entities)
            entity.Update(dt);

        _camera.Position = Vector2.Lerp(_camera.Position, _player.Character.Position, 0.05f);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _gameRenderer.Begin(_camera);

        _mapRenderer.Draw(_gameRenderer);

        foreach (var entity in _entities)
            entity.Draw(_gameRenderer);

        // _npc.Draw(_gameRenderer);
        // _player.Draw(_gameRenderer);

        _gameRenderer.End();
    }
}
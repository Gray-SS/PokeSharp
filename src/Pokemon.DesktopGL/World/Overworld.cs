using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core.Renderers;
using Pokemon.DesktopGL.Players;

namespace Pokemon.DesktopGL.World;

public sealed class Overworld
{
    public Player Player { get; private set; }

    public GameMap Map => _map;
    public IReadOnlyList<WorldEntity> Entities => _entities;

    private readonly GameMap _map;
    private readonly TiledMapRenderer _mapRenderer;

    private readonly EntitySpawner _spawner;
    private readonly List<WorldEntity> _entities;

    public Overworld(GameMap map)
    {
        _spawner = new EntitySpawner(PokemonGame.Instance.CharacterRegistry);
        _entities = new List<WorldEntity>();

        _map = map;
        _mapRenderer = new TiledMapRenderer(_map);
    }

    public bool CanMove(Character character, Vector2 targetPosition)
    {
        (int Col, int Row) = _map.GetCoord(targetPosition);

        if (_map.CollideAt(Col, Row))
            return false;

        foreach (var otherEntity in _entities)
        {
            Character otherCharacter = otherEntity.Character;
            if (character == otherCharacter)
                continue;

            (int OtherCol, int OtherRow) = _map.GetCoord(otherCharacter.Position);
            if (Col == OtherCol && Row == OtherRow)
                return false;

            (OtherCol, OtherRow) = _map.GetCoord(otherCharacter.TargetPosition);
            if (Col == OtherCol && Row == OtherRow)
                return false;
        }

        return true;
    }

    public void LoadEntities()
    {
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
                        Player = player;

                    _entities.Add(entity);
                }
            }
        }
    }

    public void Update(float dt)
    {
        foreach (var entity in _entities)
            entity.Update(dt);
    }

    public void Draw(GameRenderer renderer)
    {
        _mapRenderer.Draw(renderer);

        foreach (var entity in _entities)
            entity.Draw(renderer);
    }
}
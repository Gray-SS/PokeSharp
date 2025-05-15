using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Characters;
using PokeSharp.Engine;
using PokeSharp.Engine.Extensions;
using PokeSharp.Engine.Renderers;
using Pokemon.DesktopGL.Creatures;
using Pokemon.DesktopGL.Miscellaneous;
using Pokemon.DesktopGL.Players;

namespace Pokemon.DesktopGL.World;

public sealed class Overworld
{
    public Player Player { get; private set; }

    public GameMap Map => _map;
    public IReadOnlyList<Entity> Entities => _entities;

    private readonly GameMap _map;
    private readonly TiledMapRenderer _mapRenderer;

    private readonly WorldLoader _loader;
    private readonly List<Entity> _entities;
    private readonly Dictionary<string, CreatureZone> _zones;

    public Overworld(GameMap map)
    {
        _entities = new List<Entity>();
        _loader = new WorldLoader(map.TiledMap);
        _zones = new Dictionary<string, CreatureZone>();

        _map = map;
        _mapRenderer = new TiledMapRenderer(_map);
    }

    public void Load()
    {
        _entities.Clear();
        _entities.AddRange(_loader.LoadEntities());

        _zones.Clear();
        var zones = _loader.LoadZones();
        foreach (var zone in zones)
            _zones.Add(zone.Key, zone.Value);

        Player foundPlayer = _entities.FirstOrDefault(x => x is Player) as Player ?? throw new InvalidOperationException("No player was found in the map.");
        Player = foundPlayer;
    }

    public CreatureZone GetCurrentZone()
    {
        var objectLayers = _map.TiledMap.Layers.OfType<DotTiled.ObjectLayer>();
        foreach (DotTiled.ObjectLayer layer in objectLayers)
        {
            foreach (DotTiled.RectangleObject rectObj in layer.Objects.OfType<DotTiled.RectangleObject>())
            {
                if (rectObj.Type != "zone")
                    continue;

                int worldX = (int)(rectObj.X * GameConstants.TileSize / _map.TiledMap.TileWidth);
                int worldY = (int)(rectObj.Y * GameConstants.TileSize / _map.TiledMap.TileWidth);
                int worldWidth = (int)(rectObj.Width * GameConstants.TileSize / _map.TiledMap.TileWidth);
                int worldHeight = (int)(rectObj.Height * GameConstants.TileSize / _map.TiledMap.TileWidth);
                Rectangle bounds = new(worldX, worldY, worldWidth, worldHeight);

                if (Player.Character.Bounds.Intersects(bounds))
                {
                    var name = rectObj.GetStringRequired("name");
                    return _zones[name];
                }
            }
        }

        return null;
    }

    public bool TryGetEntityAt(Vector2 position, out Entity entity)
    {
        entity = null;
        (int Col, int Row) = Utils.ConvertWorldPosToMapCoord(position);

        foreach (var otherEntity in _entities)
        {
            Character otherCharacter = otherEntity.Character;
            (int OtherCol, int OtherRow) = Utils.ConvertWorldPosToMapCoord(otherCharacter.Position);
            if (Col == OtherCol && Row == OtherRow)
            {
                entity = otherEntity;
                return true;
            }

            (OtherCol, OtherRow) = Utils.ConvertWorldPosToMapCoord(otherCharacter.TargetPosition);
            if (Col == OtherCol && Row == OtherRow)
            {
                entity = otherEntity;
                return true;
            }
        }

        return false;
    }

    public bool IsInLeaf(Character character)
    {
        (int Col, int Row) = Utils.ConvertWorldPosToMapCoord(character.Position);
        return _map.GetData("Grass", Col, Row) == 6;
    }

    public bool CanMove(Character character, Vector2 targetPosition)
    {
        (int Col, int Row) = Utils.ConvertWorldPosToMapCoord(targetPosition);

        if (_map.CollideAt(Col, Row))
            return false;

        foreach (var otherEntity in _entities)
        {
            Character otherCharacter = otherEntity.Character;
            if (character == otherCharacter)
                continue;

            (int OtherCol, int OtherRow) = Utils.ConvertWorldPosToMapCoord(otherCharacter.Position);
            if (Col == OtherCol && Row == OtherRow)
                return false;

            (OtherCol, OtherRow) = Utils.ConvertWorldPosToMapCoord(otherCharacter.TargetPosition);
            if (Col == OtherCol && Row == OtherRow)
                return false;
        }

        return true;
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
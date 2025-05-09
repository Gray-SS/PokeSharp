using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core.Extensions;
using Pokemon.DesktopGL.Creatures;
using Pokemon.DesktopGL.Miscellaneous;
using Pokemon.DesktopGL.Patrol;

namespace Pokemon.DesktopGL.World;

public class WorldLoader
{
    private readonly DotTiled.Map _map;
    private readonly EntitySpawner _spawner;

    private readonly IEnumerable<DotTiled.ObjectLayer> _objectLayers;

    public WorldLoader(DotTiled.Map tiledMap)
    {
        _map = tiledMap;
        _spawner = new EntitySpawner(PokemonGame.Instance.CharacterRegistry);

        _objectLayers = _map.Layers.OfType<DotTiled.ObjectLayer>();
    }

    public Dictionary<string, CreatureZone> LoadZones()
    {
        var zones = new Dictionary<string, CreatureZone>();

        foreach (DotTiled.ObjectLayer objectLayer in _objectLayers)
        {
            foreach (DotTiled.RectangleObject obj in objectLayer.Objects.OfType<DotTiled.RectangleObject>())
            {
                if (obj.Type != "zone")
                    continue;

                var name = obj.GetStringRequired("name");
                var creaturesJson = obj.GetStringRequired("creatures");
                var creatures = JsonSerializer.Deserialize<CreatureSpawnEntry[]>(creaturesJson);
                var creatureRegistry = PokemonGame.Instance.CreatureRegistry;

                foreach (CreatureSpawnEntry creature in creatures)
                    creature.CreatureData = creatureRegistry.GetData(creature.CreatureId);

                var zone = new CreatureZone
                {
                    Name = name,
                    Creatures = creatures
                };

                zones.Add(name, zone);
            }
        }

        return zones;
    }

    public List<Entity> LoadEntities()
    {
        int cid = 0;
        var entities = new List<Entity>();

        foreach (DotTiled.ObjectLayer objectLayer in _objectLayers)
        {
            foreach (DotTiled.PointObject obj in objectLayer.Objects.OfType<DotTiled.PointObject>())
            {
                if (!obj.Visible || obj.Type != "entity")
                    continue;

                try
                {
                    string type = obj.GetStringRequired("type");
                    string character_id = obj.GetStringRequired("character_id");

                    (int col, int row) = Utils.ConvertMapPosToTileCoord(new Vector2(obj.X, obj.Y));
                    string id = obj.GetStringOrDefault("id", $"entity_{++cid}");

                    EntityDefinition entityDef;
                    EntityType entityType = Enum.Parse<EntityType>(type, true);

                    switch (entityType)
                    {
                        case EntityType.NPC:
                        case EntityType.Trainer:
                            string name = obj.GetStringRequired("name");
                            string dialogues = obj.GetStringOrDefault("dialogues", string.Empty);
                            string patrolId = obj.GetStringOrDefault("patrol_id", string.Empty);

                            PatrolPath patrolPath = LoadPatrolPath(obj);
                            entityDef = new EntityDefinition
                            {
                                Id = id,
                                Name = name,
                                Dialogues = string.IsNullOrEmpty(dialogues) ? [] : dialogues.Split(';'),
                                PatrolPath = patrolPath,
                                SpawnCol = col,
                                SpawnRow = row,
                                Type = entityType,
                                CharacterId = character_id
                            };
                            break;

                        case EntityType.Player:
                            entityDef = new EntityDefinition
                            {
                                Id = id,
                                SpawnCol = col,
                                SpawnRow = row,
                                Type = entityType,
                                CharacterId = character_id
                            };
                            break;

                        default:
                            throw new NotSupportedException($"The type of entity '{entityType}' is not supported");
                    }

                    var entity = _spawner.Spawn(entityDef);
                    entities.Add(entity);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while loading entity at {obj.X},{obj.Y}: {ex.Message}");
                }
            }
        }

        return entities;
    }

    private PatrolPath LoadPatrolPath(DotTiled.Object obj)
    {
        if (!obj.TryGetProperty<DotTiled.ObjectProperty>("patrol_path", out var property))
            return null;

        var patrolPathObj = _map.Layers.OfType<DotTiled.ObjectLayer>()
                                    .SelectMany(x => x.Objects)
                                    .FirstOrDefault(x => x.ID == property.Value);

        if (patrolPathObj == null)
            throw new InvalidOperationException($"The patrol path wasn't found.");

        float waitDelay = 2.0f;
        if (patrolPathObj.TryGetProperty<DotTiled.FloatProperty>("wait_delay", out var waitDelayProp))
            waitDelay = waitDelayProp.Value;

        Vector2[] patrolPoints;
        if (patrolPathObj is DotTiled.PolylineObject polylineObj)
        {
            patrolPoints = polylineObj.Points.Select(x => new Vector2(obj.X + x.X, obj.Y + x.Y)).ToArray();
        }
        else if (patrolPathObj is DotTiled.PolygonObject polygonObj)
        {
            patrolPoints = polygonObj.Points.Select(x => new Vector2(obj.X + x.X, obj.Y + x.Y)).ToArray();
        }
        else
            throw new NotSupportedException("The patrol path MUST reference a polyline obj or a polygon obj");

        return new PatrolPath
        {
            WaitDelay = waitDelay,
            PatrolPoints = patrolPoints
        };
    }
}
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.NPCs;
using Pokemon.DesktopGL.Players;

namespace Pokemon.DesktopGL.World;

public sealed class EntitySpawner
{
    private readonly CharacterRegistry _registry;

    public EntitySpawner(CharacterRegistry registry)
    {
        _registry = registry;
    }

    public WorldEntity Spawn(EntityDefinition entity)
    {
        Vector2 position = new Vector2(entity.SpawnCol, entity.SpawnRow) * GameConstants.TileSize;

        CharacterData charData = _registry.Get(entity.CharacterId);
        Character character = new(charData, position);

        switch (entity.Type)
        {
            case EntityType.Player:
                return new Player(character);
            case EntityType.NPC:
                var npcData = new NPCData
                {
                    Name = entity.Name,
                    Dialogues = entity.Dialogues
                };

                return new NPC(npcData, character);
        }

        return null;
    }

    public NPC SpawnNPC(string characterId, Vector2 position, NPCData data)
    {
        CharacterData charData = _registry.Get(characterId);
        Character character = new(charData, position);

        return new NPC(data, character);
    }

    public Player SpawnPlayer(string characterId, Vector2 position)
    {
        CharacterData data = _registry.Get(characterId);
        Character character = new(data, position);

        return new Player(character);
    }
}
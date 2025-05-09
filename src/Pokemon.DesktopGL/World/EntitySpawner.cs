using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.Miscellaneous;
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

    public WorldEntity Spawn(EntityDefinition entityDef)
    {
        Vector2 position = Utils.ConvertTileCoordToWorldPos((entityDef.SpawnCol, entityDef.SpawnRow));

        CharacterData charData = _registry.Get(entityDef.CharacterId);
        Character character = new(charData, position);

        switch (entityDef.Type)
        {
            case EntityType.Player:
                return new Player(character);
            case EntityType.NPC:
                var npcData = new NPCData
                {
                    Name = entityDef.Name,
                    Dialogues = entityDef.Dialogues,
                    PatrolPath = entityDef.PatrolPath
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
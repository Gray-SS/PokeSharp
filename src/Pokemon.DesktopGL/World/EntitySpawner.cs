using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Characters;
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

    public Entity Spawn(EntityDefinition entityDef)
    {
        Vector2 position = Utils.ConvertTileCoordToWorldPos((entityDef.SpawnCol, entityDef.SpawnRow));

        CharacterData charData = _registry.Get(entityDef.CharacterId);
        Character character = new(charData, position);

        return entityDef.Type switch
        {
            EntityType.Player => new Player(character),
            EntityType.NPC => new NPC(character, entityDef.Name, entityDef.Dialogues, entityDef.PatrolPath),
            _ => null,
        };
    }
}
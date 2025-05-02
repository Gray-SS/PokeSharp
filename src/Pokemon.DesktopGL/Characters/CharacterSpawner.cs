using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.NPCs;
using Pokemon.DesktopGL.Players;

namespace Pokemon.DesktopGL.Characters;

public sealed class CharacterSpawner
{
    private readonly CharacterRegistry _registry;

    public CharacterSpawner(CharacterRegistry registry)
    {
        _registry = registry;
    }

    public NPC SpawnNPC(string characterId, Vector2 position)
    {
        CharacterData data = _registry.Get(characterId);
        Character character = new(data, position);

        return new NPC(character);
    }

    public Player SpawnPlayer(string characterId, Vector2 position)
    {
        CharacterData data = _registry.Get(characterId);
        Character character = new(data, position);

        return new Player(character);
    }
}
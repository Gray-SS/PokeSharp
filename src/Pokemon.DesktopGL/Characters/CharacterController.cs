using Pokemon.DesktopGL.World;

namespace Pokemon.DesktopGL.Characters;

// TODO: Just realised I need to move this controller onto the World folders and rename it EntityController
//       because this will never control other things than a specific entity
public abstract class CharacterController
{
    public Character Character { get; }
    public WorldEntity Entity { get; }

    public CharacterController(WorldEntity entity)
    {
        Entity = entity;
        Character = entity.Character;
    }

    public abstract void Update(float dt);
}
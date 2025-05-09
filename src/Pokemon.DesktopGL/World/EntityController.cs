using Pokemon.DesktopGL.Characters;

namespace Pokemon.DesktopGL.World;

public abstract class EntityController
{
    public Character Character { get; }
    public Entity Entity { get; }

    public EntityController(Entity entity)
    {
        Entity = entity;
        Character = entity.Character;
    }

    public abstract void Update(float dt);
}
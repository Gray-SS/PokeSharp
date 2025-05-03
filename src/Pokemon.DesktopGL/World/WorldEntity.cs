using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core.Renderers;

namespace Pokemon.DesktopGL.World;

public abstract class WorldEntity
{
    public Character Character { get; }
    public CharacterRenderer Renderer { get; }

    public WorldEntity(Character character)
    {
        Character = character;
        Renderer = new CharacterRenderer(Character);
    }

    public virtual void Update(float dt)
    {
        Character.Update(dt);
        Renderer.Update(dt);
    }

    public void Draw(GameRenderer renderer)
    {
        Renderer.Draw(renderer);
    }

    public abstract void Interact();
}
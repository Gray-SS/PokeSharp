using Pokemon.DesktopGL.Core.Renderers;

namespace Pokemon.DesktopGL.Characters;

public abstract class CharacterEntity
{
    public Character Character { get; }
    public CharacterRenderer Renderer { get; }

    public CharacterEntity(Character character)
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
}
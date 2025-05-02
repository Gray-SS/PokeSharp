namespace Pokemon.DesktopGL.Characters;

public abstract class CharacterController
{
    public Character Character { get; }

    public CharacterController(Character character)
    {
        Character = character;
    }

    public abstract void Update(float dt);
}
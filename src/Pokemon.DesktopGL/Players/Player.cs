using Pokemon.DesktopGL.Characters;

namespace Pokemon.DesktopGL.Players;

public sealed class Player : CharacterEntity
{
    private readonly PlayerController _controller;

    public Player(Character character) : base(character)
    {
        _controller = new PlayerController(character);
    }

    public override void Update(float dt)
    {
        _controller.Update(dt);
        base.Update(dt);
    }
}
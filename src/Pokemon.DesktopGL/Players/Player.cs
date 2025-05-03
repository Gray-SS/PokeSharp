using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.World;

namespace Pokemon.DesktopGL.Players;

public sealed class Player : WorldEntity
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

    public override void Interact()
    {
    }
}
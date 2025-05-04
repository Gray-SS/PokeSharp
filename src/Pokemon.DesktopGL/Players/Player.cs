using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.Core.Extensions;
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
        var inputManager = PokemonGame.Instance.InputManager;

        if (PokemonGame.Instance.DialogueManager.IsActive)
        {
            if (inputManager.IsKeyPressed(Keys.E))
                PokemonGame.Instance.DialogueManager.SkipOrNext();

            return;
        }
        else if (Character.IsIdle && inputManager.IsKeyPressed(Keys.E))
            Interact();

        _controller.Update(dt);

        base.Update(dt);
    }

    public override void Interact()
    {
        var world = PokemonGame.Instance.ActiveWorld;

        var forwardPos = Character.Position + Character.Direction.ToVector2() * GameConstants.TileSize;
        if (!world.TryGetEntityAt(forwardPos, out WorldEntity entity))
            return;

        if (entity == this)
            return;

        entity.Character.Rotate(Character.Direction.Opposite());
        entity.Interact();
    }
}
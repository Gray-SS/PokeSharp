using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Characters;
using PokeSharp.Engine;
using PokeSharp.Engine.Extensions;
using Pokemon.DesktopGL.World;

namespace Pokemon.DesktopGL.Players;

public sealed class Player : Entity
{
    private readonly PlayerController _controller;

    public Player(Character character) : base(character)
    {
        _controller = new PlayerController(this);
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
        if (!world.TryGetEntityAt(forwardPos, out Entity entity))
            return;

        if (entity == this)
            return;

        entity.Character.Rotate(Character.Direction.Opposite());
        entity.Interact();
    }
}
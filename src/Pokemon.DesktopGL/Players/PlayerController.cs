using System;
using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core;

namespace Pokemon.DesktopGL.Players;

public class PlayerController : CharacterController
{
    public PlayerController(Character character) : base(character)
    {
    }

    public override void Update(float dt)
    {
        var inputManager = PokemonGame.Instance.InputManager;

        if (Character.IsMoving && Character.IsNearTargetPos)
        {
            if (inputManager.IsKeyDown(Keys.Up) || inputManager.IsKeyDown(Keys.W)) Character.Premove(Direction.Up);
            else if (inputManager.IsKeyDown(Keys.Down) || inputManager.IsKeyDown(Keys.S)) Character.Premove(Direction.Down);
            else if (inputManager.IsKeyDown(Keys.Left) || inputManager.IsKeyDown(Keys.A)) Character.Premove(Direction.Left);
            else if (inputManager.IsKeyDown(Keys.Right) || inputManager.IsKeyDown(Keys.D)) Character.Premove(Direction.Right);
        }
        else
        {
            Direction? targetDirection = null;

            if (inputManager.IsKeyDown(Keys.Up) || inputManager.IsKeyDown(Keys.W)) targetDirection = Direction.Up;
            else if (inputManager.IsKeyDown(Keys.Down) || inputManager.IsKeyDown(Keys.S)) targetDirection = Direction.Down;
            else if (inputManager.IsKeyDown(Keys.Left) || inputManager.IsKeyDown(Keys.A)) targetDirection = Direction.Left;
            else if (inputManager.IsKeyDown(Keys.Right) || inputManager.IsKeyDown(Keys.D)) targetDirection = Direction.Right;

            if (targetDirection == Character.Direction)
            {
                Character.Move(targetDirection.Value);
            }
            else if (targetDirection != null)
            {
                Character.Rotate(targetDirection.Value);
            }
        }
    }
}

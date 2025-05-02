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
            if (inputManager.IsKeyDown(Keys.Up)) Character.Premove(Direction.Up);
            else if (inputManager.IsKeyDown(Keys.Down)) Character.Premove(Direction.Down);
            else if (inputManager.IsKeyDown(Keys.Left)) Character.Premove(Direction.Left);
            else if (inputManager.IsKeyDown(Keys.Right)) Character.Premove(Direction.Right);
        }
        else
        {
            if (inputManager.IsKeyDown(Keys.Up)) Character.Move(Direction.Up);
            else if (inputManager.IsKeyDown(Keys.Down)) Character.Move(Direction.Down);
            else if (inputManager.IsKeyDown(Keys.Left)) Character.Move(Direction.Left);
            else if (inputManager.IsKeyDown(Keys.Right)) Character.Move(Direction.Right);
        }
    }
}

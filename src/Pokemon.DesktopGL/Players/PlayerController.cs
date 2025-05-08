using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core;

namespace Pokemon.DesktopGL.Players;

public class PlayerController : CharacterController
{
    private static readonly (Direction dir, Keys[] keys)[] _inputKeys = [
        (Direction.Left, [ Keys.Left, Keys.A ]),
        (Direction.Right, [ Keys.Right, Keys.D ]),
        (Direction.Up, [ Keys.Up, Keys.W ]),
        (Direction.Down, [ Keys.Down, Keys.S ]),
    ];

    public PlayerController(Character character) : base(character)
    {
    }

    public override void Update(float dt)
    {
        var inputManager = PokemonGame.Instance.InputManager;

        if (Character.IsMoving && this.IsNearLastTargetPosition())
        {
            foreach ((Direction dir, Keys[] keys) in _inputKeys)
            {
                if (keys.Any(inputManager.IsKeyDown))
                {
                    Character.QueueMove(dir);
                    break;
                }
            }
        }
        else if (Character.IsIdle)
        {
            Direction? targetDirection = null;

            foreach ((Direction dir, Keys[] keys) in _inputKeys)
            {
                if (keys.Any(inputManager.IsKeyDown))
                {
                    targetDirection = dir;
                    break;
                }
            }

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

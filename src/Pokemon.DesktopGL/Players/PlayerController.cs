using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.Core.Managers;

namespace Pokemon.DesktopGL.Players
{
    public class PlayerController : CharacterController
    {
        private Direction? _lastPressedDirection = null;

        private static readonly (Direction Direction, Keys[] Keys)[] _inputKeys =
        [
            (Direction.Left, [Keys.Left, Keys.A]),
            (Direction.Right, [Keys.Right, Keys.D]),
            (Direction.Up, [Keys.Up, Keys.W]),
            (Direction.Down, [Keys.Down, Keys.S])
        ];

        public PlayerController(Player player) : base(player)
        {
        }

        public override void Update(float dt)
        {
            var inputManager = PokemonGame.Instance.InputManager;

            UpdateLastPressedDirection(inputManager);

            if (Character.IsMoving && Character.GetMoveProgress() >= 0.5f)
            {
                TryPremoveFromInput(inputManager);
            }
            else if (Character.IsIdle)
            {
                HandleIdleMovement(inputManager);
            }
        }

        private void UpdateLastPressedDirection(InputManager inputManager)
        {
            foreach (var (direction, keys) in _inputKeys)
            {
                foreach (var key in keys)
                {
                    if (inputManager.IsKeyPressed(key))
                    {
                        _lastPressedDirection = direction;
                        return;
                    }
                }
            }
        }

        private void TryPremoveFromInput(InputManager inputManager)
        {
            if (_lastPressedDirection.HasValue && IsAnyKeyForDirectionPressed(inputManager, _lastPressedDirection.Value))
            {
                Character.SetPremove(_lastPressedDirection.Value);
                return;
            }

            foreach (var (direction, keys) in _inputKeys)
            {
                if (keys.Any(inputManager.IsKeyDown))
                {
                    Character.SetPremove(direction);
                    break;
                }
            }
        }

        // private bool IsPremoveReady()
        // {
        //     if (!Character.IsMoving)
        //         return false;

        //     return Vector2.Distance(Character.Position, lastTargetPos) <= GameConstants.TileSize * 0.5f;
        // }

        private void HandleIdleMovement(InputManager inputManager)
        {
            Direction? targetDirection;

            if (_lastPressedDirection.HasValue && IsAnyKeyForDirectionPressed(inputManager, _lastPressedDirection.Value))
            {
                targetDirection = _lastPressedDirection.Value;
            }
            else
            {
                targetDirection = GetDirectionFromInput(inputManager);
            }

            if (targetDirection == null)
                return;

            if (targetDirection == Character.Direction)
                Character.Move(targetDirection.Value);
            else
                Character.Rotate(targetDirection.Value);
        }

        private static Direction? GetDirectionFromInput(InputManager inputManager)
        {
            foreach (var (dir, keys) in _inputKeys)
            {
                if (keys.Any(inputManager.IsKeyDown))
                {
                    return dir;
                }
            }
            return null;
        }

        private static bool IsAnyKeyForDirectionPressed(InputManager inputManager, Direction direction)
        {
            foreach (var (dir, keys) in _inputKeys)
            {
                if (dir == direction && keys.Any(inputManager.IsKeyDown))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
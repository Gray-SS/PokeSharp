using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Extensions;

public static class DirectionExtensions
{
    public static Vector2 ToVector2(this Direction direction)
    {
        return direction switch
        {
            Direction.Left => new Vector2(-1.0f, 0.0f),
            Direction.Right => new Vector2(1.0f, 0.0f),
            Direction.Up => new Vector2(0.0f, -1.0f),
            Direction.Down => new Vector2(0.0f, 1.0f),
            _ => Vector2.Zero
        };
    }

    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            _ => direction
        };
    }
}
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core;

namespace Pokemon.DesktopGL.Characters;

public class Character
{
    public const float Speed = 300.0f;

    public CharacterData Data { get; }
    public Vector2 Position { get; private set; }
    public Vector2 Size { get; set; } = new Vector2(50, 75);
    public Vector2 TargetPosition { get; private set; }
    public Direction Direction { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsNearTargetPos => Vector2.Distance(Position, TargetPosition) <= GameConstants.TileSize * 0.4f;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

    private bool _premoved;
    private Vector2 _nextTargetPosition;
    private Direction _nextDirection;

    public Character(CharacterData data, Vector2 startPosition)
    {
        Data = data;
        Position = startPosition;
        TargetPosition = startPosition;
        Direction = Direction.Down;
    }

    public void Move(Direction direction)
    {
        if (IsMoving) return;

        Direction = direction;
        IsMoving = true;

        TargetPosition = direction switch
        {
            Direction.Left => Position - new Vector2(GameConstants.TileSize, 0),
            Direction.Right => Position + new Vector2(GameConstants.TileSize, 0),
            Direction.Up => Position - new Vector2(0, GameConstants.TileSize),
            Direction.Down => Position + new Vector2(0, GameConstants.TileSize),
            _ => Position
        };
    }

    public void Premove(Direction direction)
    {
        _premoved = true;
        _nextDirection = direction;

        _nextTargetPosition = direction switch
        {
            Direction.Left => TargetPosition - new Vector2(GameConstants.TileSize, 0),
            Direction.Right => TargetPosition + new Vector2(GameConstants.TileSize, 0),
            Direction.Up => TargetPosition - new Vector2(0, GameConstants.TileSize),
            Direction.Down => TargetPosition + new Vector2(0, GameConstants.TileSize),
            _ => TargetPosition
        };
    }

    public void Update(float dt)
    {
        if (!IsMoving) return;

        float distSq = Vector2.DistanceSquared(Position, TargetPosition);
        if (distSq >= 2f)
        {
            Vector2 dir = TargetPosition - Position;
            dir.Normalize();
            Position += dir * Speed * dt;
        }
        else
        {
            Position = TargetPosition;

            if (_premoved)
            {
                Direction = _nextDirection;
                TargetPosition = _nextTargetPosition;
                _premoved = false;
            }
            else IsMoving = false;
        }
    }
}

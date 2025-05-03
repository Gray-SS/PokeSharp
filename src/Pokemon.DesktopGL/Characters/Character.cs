using System;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core;

namespace Pokemon.DesktopGL.Characters;

public enum CharacterState
{
    Idle,
    Moving,
    Rotating,
}

public class Character
{
    public const float Speed = 300.0f;

    public CharacterData Data { get; }
    public Vector2 Position { get; private set; }
    public Vector2 Size { get; set; } = new Vector2(50, 75);
    public Vector2 TargetPosition { get; private set; }
    public Direction Direction { get; private set; }
    public CharacterState State { get; private set; }
    public bool IsMoving => State == CharacterState.Moving;
    public bool IsRotating => State == CharacterState.Rotating;
    public bool IsIdle => State == CharacterState.Idle;
    public bool IsNearTargetPos => Vector2.Distance(Position, TargetPosition) <= GameConstants.TileSize * 0.4f;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

    public event EventHandler Rotated;

    private bool _premoved;
    private Vector2 _nextTargetPosition;
    private Direction _nextDirection;

    private float _rotatingTimer;

    public Character(CharacterData data, Vector2 startPosition)
    {
        Data = data;
        Position = startPosition;
        TargetPosition = startPosition;
        Direction = Direction.Down;
    }

    public void Move(Direction direction)
    {
        if (State != CharacterState.Idle) return;

        Direction = direction;
        State = CharacterState.Moving;

        TargetPosition = direction switch
        {
            Direction.Left => Position - new Vector2(GameConstants.TileSize, 0),
            Direction.Right => Position + new Vector2(GameConstants.TileSize, 0),
            Direction.Up => Position - new Vector2(0, GameConstants.TileSize),
            Direction.Down => Position + new Vector2(0, GameConstants.TileSize),
            _ => Position
        };
    }

    public void Rotate(Direction direction)
    {
        if (State != CharacterState.Idle) return;

        Direction = direction;
        State = CharacterState.Rotating;
        Rotated?.Invoke(this, EventArgs.Empty);

        _rotatingTimer = 0.0f;
    }

    public void Premove(Direction direction)
    {
        if (State != CharacterState.Moving) return;

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
        switch (State)
        {
            case CharacterState.Idle: return;
            case CharacterState.Moving:
                HandleMovingState(dt);
                return;
            case CharacterState.Rotating:
                HandleRotatingState(dt);
                return;
        }
    }

    private void HandleMovingState(float dt)
    {
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
            else State = CharacterState.Idle;
        }
    }

    private void HandleRotatingState(float dt)
    {
        _rotatingTimer += dt;
        if (_rotatingTimer >= 0.2f)
        {
            State = CharacterState.Idle;
            _rotatingTimer = 0.0f;
        }
    }
}

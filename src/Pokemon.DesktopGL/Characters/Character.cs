using System;
using System.Collections.Generic;
using System.Linq;
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
    public Vector2 TargetPosition => _queuedMoves.Count > 0 ? _queuedMoves.Peek().TargetPos : Position;
    public Direction TargetDirection => _queuedMoves.Count > 0 ? _queuedMoves.Peek().TargetDir : Direction;
    public Direction Direction { get; private set; }
    public CharacterState State { get; private set; }
    public bool IsMoving => State == CharacterState.Moving;
    public bool IsRotating => State == CharacterState.Rotating;
    public bool IsIdle => State == CharacterState.Idle;
    public bool MovementEnabled { get; set; } = true;
    public bool RotationEnabled { get; set; } = true;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

    public event EventHandler Moved;
    public event EventHandler Rotated;

    public IReadOnlyCollection<QueuedMove> QueuedMoves => _queuedMoves;

    private readonly Queue<QueuedMove> _queuedMoves;

    private float _rotatingTimer;

    public Character(CharacterData data, Vector2 startPosition)
    {
        Data = data;
        Position = startPosition;

        _queuedMoves = new Queue<QueuedMove>();
    }

    public void Move(Direction direction)
    {
        State = CharacterState.Moving;
        QueueMove(direction);
    }

    public void Stop()
    {
        _queuedMoves.Clear();
    }

    public bool CanMove(Vector2 targetPos)
    {
        if (!MovementEnabled) return false;
        return PokemonGame.Instance.ActiveWorld.CanMove(this, targetPos);
    }

    public void Rotate(Direction direction, bool force = false)
    {
        if (!force && (!RotationEnabled || !IsIdle || Direction == direction)) return;

        Direction = direction;
        State = CharacterState.Rotating;
        Rotated?.Invoke(this, EventArgs.Empty);

        _rotatingTimer = 0.0f;
    }

    public void QueueMove(Direction direction)
    {
        Vector2 basePosition = _queuedMoves.Count > 0 ? _queuedMoves.Last().TargetPos : Position;
        Vector2 targetPosition = CalcTargetPosition(basePosition, direction);

        if (!CanMove(targetPosition))
            return;

        _queuedMoves.Enqueue(new QueuedMove
        {
            TargetDir = direction,
            TargetPos = targetPosition,
        });

        if (IsIdle)
            State = CharacterState.Moving;
    }

    public void Update(float dt)
    {
        switch (State)
        {
            case CharacterState.Moving:
                HandleMovingState(dt);
                return;
            case CharacterState.Rotating:
                HandleRotatingState(dt);
                return;

            default: return;
        }
    }

    private void HandleMovingState(float dt)
    {
        if (!MovementEnabled || _queuedMoves.Count <= 0)
        {
            State = CharacterState.Idle;
            return;
        }

        Direction = TargetDirection;
        Vector2 dir = TargetPosition - Position;
        float distance = dir.Length();

        float moveStep = Speed * dt;

        if (distance > moveStep)
        {
            dir.Normalize();
            Position += dir * Speed * dt;
        }
        else
        {
            Position = new Vector2(
                MathF.Round(TargetPosition.X),
                MathF.Round(TargetPosition.Y));

            Moved?.Invoke(this, EventArgs.Empty);

            if (_queuedMoves.Count > 0) _queuedMoves.Dequeue();
            else State = CharacterState.Idle;
        }
    }

    private void HandleRotatingState(float dt)
    {
        _rotatingTimer += dt;
        if (_rotatingTimer >= 0.15f)
        {
            State = CharacterState.Idle;
            _rotatingTimer = 0.0f;
        }
    }

    private static Vector2 CalcTargetPosition(Vector2 basePosition, Direction targetDir)
    {
        return targetDir switch
        {
            Direction.Left => basePosition - new Vector2(GameConstants.TileSize, 0),
            Direction.Right => basePosition + new Vector2(GameConstants.TileSize, 0),
            Direction.Up => basePosition - new Vector2(0, GameConstants.TileSize),
            Direction.Down => basePosition + new Vector2(0, GameConstants.TileSize),
            _ => basePosition
        };
    }

    public readonly struct QueuedMove
    {
        public required readonly Vector2 TargetPos { get; init; }
        public required readonly Direction TargetDir { get; init; }
    }
}

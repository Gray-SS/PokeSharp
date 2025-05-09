using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.Core.Extensions;

namespace Pokemon.DesktopGL.Characters;

public enum CharacterState
{
    Idle,
    Moving,
    Rotating,
}

public readonly struct QueuedMove
{
    public required readonly Vector2 TargetPos { get; init; }
    public required readonly Direction TargetDir { get; init; }
}

public class Character
{
    public const float Speed = 300.0f;

    public CharacterData Data { get; }
    public Vector2 Position { get; private set; }
    public Vector2 StartPosition { get; private set; }
    public Vector2 Size { get; set; } = new Vector2(50, 75);
    public QueuedMove? CurrentMove => _currentMove;
    public QueuedMove? PremoveMove => _premoveMove;
    public Vector2 TargetPosition => _currentMove?.TargetPos ?? Position;
    public Direction TargetDirection => _currentMove?.TargetDir ?? Direction;
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

    private float _rotatingTimer;
    private QueuedMove? _currentMove = null;
    private QueuedMove? _premoveMove = null;

    public Character(CharacterData data, Vector2 startPosition)
    {
        Data = data;
        Position = startPosition;
    }

    public void Stop()
    {
        _currentMove = null;
        _premoveMove = null;
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

    public float GetMoveProgress()
    {
        if (!IsMoving)
            return 0f;

        float distanceTraveled = Vector2.Distance(StartPosition, Position);
        float totalDistance = Vector2.Distance(StartPosition, TargetPosition);

        if (totalDistance <= float.Epsilon)
            return 1f;

        return distanceTraveled / totalDistance;
    }

    // NOTE: Add a Move function that moves n time in a provided direction ?
    //       This could simplify the NPCController logic.
    public void Move(Direction direction)
    {
        if (!IsIdle) return;

        State = CharacterState.Moving;
        Vector2 targetPosition = CalcTargetPosition(Position, direction);
        if (!CanMove(targetPosition))
            return;

        StartPosition = Position;
        _currentMove = new QueuedMove
        {
            TargetDir = direction,
            TargetPos = targetPosition,
        };
    }

    public void SetPremove(Direction direction)
    {
        if (!IsMoving || !_currentMove.HasValue) return;

        Vector2 targetPosition = CalcTargetPosition(TargetPosition, direction);
        if (!CanMove(targetPosition))
            return;

        _premoveMove = new QueuedMove
        {
            TargetDir = direction,
            TargetPos = targetPosition,
        };
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
        if (!MovementEnabled || !_currentMove.HasValue)
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
            Position += dir * moveStep;
        }
        else
        {
            Position = new Vector2(TargetPosition.X, TargetPosition.Y);
            Moved?.Invoke(this, EventArgs.Empty);

            if (_premoveMove.HasValue)
            {
                _currentMove = _premoveMove;
                _premoveMove = null;
            }
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
        return basePosition + targetDir.ToVector2() * GameConstants.TileSize;
    }
}
using System;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.Miscellaneous;
using Pokemon.DesktopGL.Patrol;
using Pokemon.DesktopGL.World;

namespace Pokemon.DesktopGL.NPCs;

public enum NPCState
{
    Idle,
    Moving,
}

public sealed class NPCController : EntityController
{
    private int _pointIndex;
    private PatrolPath _patrolPath;
    private Vector2[] _patrolPoints;
    private NPCState _state;
    private float _timer = 0.0f;
    private readonly NPC _npc;

    public NPCController(NPC npc) : base(npc)
    {
        _npc = npc;
        _pointIndex = 0;
        _state = NPCState.Idle;
        _patrolPath = npc.PatrolPath;
        _patrolPoints = npc.PatrolPath.PatrolPoints;
    }

    public override void Update(float dt)
    {
        switch (_state)
        {
            case NPCState.Idle:
                UpdateIdleState(dt);
                break;
            case NPCState.Moving:
                UpdateMovingState();
                break;
        }
    }

    private void UpdateIdleState(float dt)
    {
        if (_patrolPath == null || _patrolPoints.Length == 0)
            return;

        _timer += dt;
        if (_timer >= _patrolPath.WaitDelay)
        {
            _timer = 0.0f;
            _state = NPCState.Moving;
        }
    }

    private void UpdateMovingState()
    {
        if (_patrolPoints.Length == 0)
            return;

        if (!Character.IsMoving)
        {
            MoveToNextPoint();
            return;
        }

        float moveProgress = Character.GetMoveProgress();
        if (moveProgress > 0.5f)
        {
            PrepareNextMove();
        }
    }

    private void MoveToNextPoint()
    {
        Vector2 patrolPoint = _patrolPoints[_pointIndex % _patrolPoints.Length];
        Vector2 patrolTargetPoint = Utils.ConvertMapPosToWorldPos(patrolPoint);

        Vector2 dirVector = patrolTargetPoint - Character.Position;
        if (dirVector.Length() <= GameConstants.TileSize * 0.1f)
        {
            _pointIndex++;
            _state = NPCState.Idle;
            return;
        }

        Direction direction = DetermineMovingDirection(dirVector);
        Character.Move(direction);
    }

    private void PrepareNextMove()
    {
        Vector2 patrolPoint = _patrolPoints[_pointIndex % _patrolPoints.Length];
        Vector2 patrolTargetPoint = Utils.ConvertMapPosToWorldPos(patrolPoint);

        Vector2 dirVector = patrolTargetPoint - Character.TargetPosition;
        if (dirVector.Length() <= GameConstants.TileSize * 0.1f)
            return;

        Character.SetPremove(Character.Direction);
    }

    private static Direction DetermineMovingDirection(Vector2 dirVector)
    {
        if (Math.Abs(dirVector.X) > Math.Abs(dirVector.Y))
            return dirVector.X > 0 ? Direction.Right : Direction.Left;
        else
            return dirVector.Y > 0 ? Direction.Down : Direction.Up;
    }
}

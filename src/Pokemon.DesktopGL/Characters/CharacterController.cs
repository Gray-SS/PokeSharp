using System.Linq;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core;

namespace Pokemon.DesktopGL.Characters;

public abstract class CharacterController
{
    public Character Character { get; }

    public CharacterController(Character character)
    {
        Character = character;
    }

    protected bool IsNearLastTargetPosition()
    {
        var queuedMoves = Character.QueuedMoves;
        if (queuedMoves.Count == 0)
            return false;

        Vector2 lastTargetPos = queuedMoves.Last().TargetPos;
        return Vector2.Distance(Character.Position, lastTargetPos) <= GameConstants.TileSize * 0.1f;
    }

    public abstract void Update(float dt);
}
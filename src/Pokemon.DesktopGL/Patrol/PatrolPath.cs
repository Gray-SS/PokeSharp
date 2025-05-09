using Microsoft.Xna.Framework;

namespace Pokemon.DesktopGL.Patrol;

public sealed class PatrolPath
{
    public required float WaitDelay { get; init; }

    public required Vector2[] PatrolPoints { get; init; }
}
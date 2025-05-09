using Pokemon.DesktopGL.Patrol;

namespace Pokemon.DesktopGL.NPCs;

public sealed class NPCData
{
    public string Name { get; init; }

    public string[] Dialogues { get; init; }

    public PatrolPath PatrolPath { get; init; }
}
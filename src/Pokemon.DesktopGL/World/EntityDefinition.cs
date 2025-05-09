using Pokemon.DesktopGL.Patrol;

namespace Pokemon.DesktopGL.World;

public sealed class EntityDefinition
{
    public required string Id { get; init; }
    public required EntityType Type { get; init; }
    public required int SpawnCol { get; init; }
    public required int SpawnRow { get; init; }
    public required string CharacterId { get; init; }
    public string Name { get; init; }
    public string[] Dialogues { get; init; }
    public PatrolPath PatrolPath { get; init; }
}
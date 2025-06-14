namespace PokeEngine.Core;

public sealed class UpdateContext
{
    public float DeltaTime { get; internal set; }
    public float TotalTime { get; internal set; }
}
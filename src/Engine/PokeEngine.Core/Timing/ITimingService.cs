namespace PokeEngine.Core.Timing;

public interface ITimingService
{
    float DeltaTime { get; }
    float TotalTime { get; }
}
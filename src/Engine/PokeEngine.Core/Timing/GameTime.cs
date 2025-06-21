using PokeCore.DependencyInjection.Abstractions;

namespace PokeEngine.Core.Timing;

public static class GameTime
{
    private static readonly ITimingService _timingService = ServiceLocator.GetRequiredService<ITimingService>();

    public static float DeltaTime => _timingService.DeltaTime;
    public static float TotalTime => _timingService.TotalTime;
}
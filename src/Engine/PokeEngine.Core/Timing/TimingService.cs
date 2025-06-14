namespace PokeEngine.Core.Timing;

public sealed class TimingService : ITimingService, IDisposable
{
    public float DeltaTime => _cachedUpdateContext?.DeltaTime ?? 0;
    public float TotalTime => _cachedUpdateContext?.TotalTime ?? 0;

    private readonly IGameLoop _gameLoop;
    private UpdateContext _cachedUpdateContext = null!;

    public TimingService(IGameLoop gameLoop)
    {
        _gameLoop = gameLoop;
        _gameLoop.Updated += Update;
    }

    public void Update(object? sender, UpdateContext context)
    {
        _cachedUpdateContext = context;
    }

    public void Dispose()
    {
        _gameLoop.Updated -= Update;
    }
}
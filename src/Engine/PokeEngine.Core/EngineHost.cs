using PokeCore.Logging;
using PokeCore.Hosting.Abstractions;

namespace PokeEngine.Core;

public sealed class EngineHost : IWaitableHostedService
{
    private readonly BaseEngine _engine;
    private readonly TaskCompletionSource _tcs;

    private readonly Logger _logger;

    public EngineHost(Logger<EngineHost> logger, BaseEngine engine)
    {
        _logger = logger;
        _tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        _engine = engine;
        _engine.Exiting += OnEngineExited;
    }

    private void OnEngineExited(object? sender, EventArgs e)
    {
        _tcs.TrySetResult();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Starting game engine...");
        Task.Run(() =>
        {
            try
            {
                _engine.Run();
            }
            catch (Exception ex)
            {
                _logger.Fatal("Unhandled exception occurred during engine execution.");
                _tcs.TrySetException(ex);
            }
            finally
            {
                _tcs.TrySetResult();
            }

        }, cancellationToken);

        _logger.Info("Game engine successfully started.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Stopping the game engine...");
        _engine.Exit();
        _logger.Info("Game engine stopped successfully.");

        return Task.CompletedTask;
    }

    public Task WaitForShutdownAsync()
    {
        return _tcs.Task;
    }
}
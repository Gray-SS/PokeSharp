using PokeCore.Logging;
using System.Collections.Concurrent;

namespace PokeLab.Presentation.Common;

internal sealed class DefaultTaskDispatcher : ITaskDispatcher, IDisposable
{
    private bool _isDisposed;
    private readonly Logger _logger;
    private readonly ITickSource _tickSource;
    private readonly ConcurrentQueue<Action> _deferredActions = new();

    public DefaultTaskDispatcher(
        ITickSource tickSource,
        Logger<DefaultTaskDispatcher> logger)
    {
        _logger = logger;

        _tickSource = tickSource;
        _tickSource.OnTick += OnTick;
    }

    private void OnTick()
    {
        while (_deferredActions.TryDequeue(out Action? action))
        {
            action.Invoke();
        }
    }

    public void FireAndForget(Action? backgroundTask)
    {
        if (backgroundTask == null)
            return;

        _ = Task.Run(backgroundTask);
        _logger.Trace($"Action fired in the background: '{backgroundTask.Method.Name}'");
    }

    public void RunOnUIThread(Action? deferredAction)
    {
        if (deferredAction == null)
            return;

        _deferredActions.Enqueue(deferredAction);
        _logger.Trace($"Operation deferred on the UI thread: '{deferredAction.Method.Name}'");
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _deferredActions.Clear();
            _tickSource.OnTick -= OnTick;

            GC.SuppressFinalize(this);
            _isDisposed = true;
        }
    }
}
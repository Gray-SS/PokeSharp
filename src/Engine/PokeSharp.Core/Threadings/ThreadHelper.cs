using System.Collections.Concurrent;

namespace PokeSharp.Core.Threadings;

public static class ThreadHelper
{
    private static readonly ConcurrentQueue<Action> _callbackActions = new();
    private static Thread? _mainThread;

    public static void Initialize()
    {
        _mainThread = Thread.CurrentThread;
    }

    public static bool IsMainThread =>
        _mainThread != null && Thread.CurrentThread == _mainThread;

    public static void EnsureMainThread()
    {
        if (!IsMainThread)
            throw new InvalidOperationException("This operation must be called from the main thread.");
    }

    public static void RunOnMainThread(Action callback)
    {
        _callbackActions.Enqueue(callback);
    }

    public static void Update()
    {
        EnsureMainThread();

        while (_callbackActions.TryDequeue(out Action? callback))
        {
            callback.Invoke();
        }
    }
}

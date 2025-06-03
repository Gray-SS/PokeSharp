using System.Collections.Concurrent;

namespace PokeSharp.Core.Threadings;

public static class ThreadDispatcher
{
    private static readonly ConcurrentQueue<Action> _callbackActions = new();

    public static void RunOnMainThread(Action callback)
    {
        if (_callbackActions.Contains(callback))
            return;

        _callbackActions.Enqueue(callback);
    }

    public static void Update()
    {
        while (_callbackActions.TryDequeue(out Action? callback))
        {
            callback.Invoke();
        }
    }
}
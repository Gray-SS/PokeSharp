using Microsoft.Xna.Framework;

namespace PokeEngine.Core.Coroutines;

public sealed class WaitUntil : ICoroutine
{
    public Func<bool> Predicate { get; }

    public WaitUntil(Func<bool> predicate)
    {
        Predicate = predicate;
    }

    public bool IsFinished(UpdateContext _)
    {
        return Predicate.Invoke();
    }
}
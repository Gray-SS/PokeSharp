using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Coroutines;

public sealed class WaitUntil : ICoroutine
{
    public Func<bool> Predicate { get; }

    public WaitUntil(Func<bool> predicate)
    {
        Predicate = predicate;
    }

    public bool IsFinished(GameTime gameTime)
    {
        return Predicate.Invoke();
    }
}
using Microsoft.Xna.Framework;

namespace PokeEngine.Core.Coroutines;

public sealed class WaitForSeconds : ICoroutine
{
    private float _elapsed;
    private float _duration;

    public WaitForSeconds(float duration)
    {
        _elapsed = 0.0f;
        _duration = duration;
    }

    public bool IsFinished(UpdateContext context)
    {
        _elapsed += context.DeltaTime;
        return _elapsed >= _duration;
    }
}
namespace Pokemon.DesktopGL.Core.Coroutines;

public sealed class WaitForSeconds : ICoroutine
{
    private float _elapsed;
    private float _duration;

    public WaitForSeconds(float duration)
    {
        _elapsed = 0.0f;
        _duration = duration;
    }

    public bool IsFinished(float dt)
    {
        _elapsed += dt;
        return _elapsed >= _duration;
    }
}
using Microsoft.Xna.Framework;

namespace PokeSharp.Engine.Core.Coroutines;

public sealed class WaitForSeconds : ICoroutine
{
    private float _elapsed;
    private float _duration;

    public WaitForSeconds(float duration)
    {
        _elapsed = 0.0f;
        _duration = duration;
    }

    public bool IsFinished(GameTime gameTime)
    {
        _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        return _elapsed >= _duration;
    }
}
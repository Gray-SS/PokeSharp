using Microsoft.Xna.Framework;

namespace PokeEngine.Core.Coroutines;

public interface ICoroutine
{
    bool IsFinished(GameTime gameTime);
}
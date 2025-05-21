using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Coroutines;

public interface ICoroutine
{
    bool IsFinished(GameTime gameTime);
}
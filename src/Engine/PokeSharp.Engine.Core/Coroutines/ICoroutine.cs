using Microsoft.Xna.Framework;

namespace PokeSharp.Engine.Core.Coroutines;

public interface ICoroutine
{
    bool IsFinished(GameTime gameTime);
}
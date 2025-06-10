using Microsoft.Xna.Framework;

namespace PokeEngine.Core;

public interface IEngineHook
{
    void Initialize();

    void Update(GameTime gameTime);

    void Draw(GameTime gameTime);
}
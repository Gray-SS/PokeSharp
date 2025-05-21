using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Services;

public interface IEngineHookDispatcher
{
    void Initialize();

    void Update(GameTime gameTime);

    void Draw(GameTime gameTime);
}
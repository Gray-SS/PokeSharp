using Microsoft.Xna.Framework;

namespace PokeSharp.Core;

public interface IEngineHook
{
    void Initialize();

    void Update(GameTime gameTime);

    void Draw(GameTime gameTime);
}
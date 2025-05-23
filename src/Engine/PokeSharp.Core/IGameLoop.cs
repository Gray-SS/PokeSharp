using Microsoft.Xna.Framework;

namespace PokeSharp.Core;

public interface IGameLoop
{
    void Initialize();

    void Update(GameTime gameTime);

    void Draw(GameTime gameTime);

    void Exit();
}
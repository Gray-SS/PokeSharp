using Microsoft.Xna.Framework;

namespace PokeEngine.Scenes;

public interface IScene
{
    

    void Load();

    void Unload();

    void Update(GameTime gameTime);

    void Draw(GameTime gameTime);
}
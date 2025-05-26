using Microsoft.Xna.Framework;

namespace PokeSharp.Scenes;

public interface IScene
{
    

    void Load();

    void Unload();

    void Update(GameTime gameTime);

    void Draw(GameTime gameTime);
}
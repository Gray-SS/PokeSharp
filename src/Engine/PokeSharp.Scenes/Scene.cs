using Microsoft.Xna.Framework;
using PokeSharp.Entities;

namespace PokeSharp.Scenes;

public sealed class Scene : IScene
{
    public string Name { get; set; }
    public List<Entity> Entities { get; set; }

    public Scene(string name)
    {
        Name = name;
        Entities = new List<Entity>();
    }

    public void Load()
    {
        foreach (Entity entity in Entities)
        {
            entity.OnLoad();
        }
    }

    public void Unload()
    {
    }

    public void Update(GameTime gameTime)
    {
        foreach (Entity entity in Entities)
        {
            entity.OnUpdate(gameTime);
        }
    }

    public void Draw(GameTime gameTime)
    {
        foreach (Entity entity in Entities)
        {
            entity.OnDraw(gameTime);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PokeEngine.Entities;

public class Entity
{
    public string Id { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public Color Color { get; set; }

    public virtual void OnLoad()
    {
    }

    public virtual void OnUpdate(GameTime gameTime)
    {

    }

    public virtual void OnDraw(GameTime gameTime)
    {
    }
}
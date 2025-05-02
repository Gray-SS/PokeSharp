using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.DesktopGL.Core;

public abstract class Screen
{
    public PokemonGame Game { get; }
    public GameWindow Window => Game.Window;
    public ContentManager Content => Game.Content;
    public GraphicsDeviceManager Graphics => Game.Graphics;
    public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

    public Screen(PokemonGame game)
    {
        Game = game;
    }

    public virtual void Load()
    {
    }

    public virtual void Unload()
    {
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw(GameTime gameTime)
    {
    }
}
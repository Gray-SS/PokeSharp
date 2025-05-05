using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.Core.Managers;
using Pokemon.DesktopGL.Core.Renderers;
using Pokemon.DesktopGL.Core.Tweening;

namespace Pokemon.DesktopGL.Core;

public abstract class Screen
{
    public PokemonGame Game { get; }
    public GameWindow Window => Game.Window;
    public ContentManager Content => Game.Content;
    public GraphicsDeviceManager Graphics => Game.Graphics;
    public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

    private float _transitionOpacity;
    private UIRenderer _renderer;

    public Screen()
    {
        Game = PokemonGame.Instance;
    }

    public IEnumerator FadeIn()
    {
        _transitionOpacity = 0.0f;
        return Tween.To((v) => _transitionOpacity = v, () => _transitionOpacity, 1f, 1f, Easing.InOutQuad);
    }

    public IEnumerator FadeOut()
    {
        _transitionOpacity = 1.0f;
        return Tween.To((v) => _transitionOpacity = v, () => _transitionOpacity, 0.0f, 1f, Easing.InOutQuad);
    }

    public virtual void Initialize()
    {
        _renderer = new UIRenderer(GraphicsDevice);
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
        _renderer.Begin();
        _renderer.Draw(Game.AssetsManager.Sprite_Blank, Game.WindowManager.Rect, Color.Black, _transitionOpacity);
        _renderer.End();
    }
}
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Engine.Managers;
using PokeSharp.Engine.Renderers;
using PokeSharp.Engine.Tweening;

namespace PokeSharp.Engine;

public abstract class Screen
{
    public PokesharpEngine Engine { get; }
    public RomManager RomManager => Engine.RomManager;
    public GameWindow Window => Engine.Window;
    public ContentManager Content => Engine.Content;
    public GraphicsDeviceManager Graphics => Engine.Graphics;
    public GraphicsDevice GraphicsDevice => Engine.GraphicsDevice;

    private float _transitionOpacity;
    private UIRenderer _renderer = null!;

    public Screen()
    {
        Engine = PokesharpEngine.Instance;
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
        _renderer.Draw(Engine.AssetsManager.Sprite_Blank, Engine.WindowManager.Rect, Color.Black, _transitionOpacity);
        _renderer.End();
    }

    protected T GetEngine<T>() where T : PokesharpEngine
        => (T)Engine;
}
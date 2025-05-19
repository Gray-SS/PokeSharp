using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Engine.Managers;
using PokeSharp.Engine.Renderers;
using PokeSharp.Engine.Tweening;
using PokeSharp.Engine.UI;

namespace PokeSharp.Engine;

public abstract class Screen
{
    public bool IsActive => Engine.ScreenManager.ActiveScreen == this;

    public PokesharpEngine Engine { get; }
    public Rectangle RenderBounds => _renderBounds;
    public UIManager UIManager => Engine.UIManager;
    public RomManager RomManager => Engine.RomManager;
    public WindowManager WindowManager => Engine.WindowManager;
    public ResolutionManager ResolutionManager => Engine.ResolutionManager;

    public GraphicsDeviceManager Graphics => Engine.Graphics;
    public GraphicsDevice GraphicsDevice => Engine.GraphicsDevice;

    private float _transitionOpacity;

    private Rectangle _renderBounds;
    private RenderTarget2D _renderTarget = null!;
    private SpriteBatch _spriteBatch = null!;

    public Screen()
    {
        Engine = PokesharpEngine.Instance;
        ResolutionManager.ResolutionChanged += OnResolutionChanged;
        ResolutionManager.VirtualResolutionChanged += OnVirtualResolutionChanged;

        CalculateRenderBounds(ResolutionManager.Resolution, ResolutionManager.VirtualResolution);
    }

    private void OnResolutionChanged(object? sender, ResolutionChangedArgs e)
    {
        Resolution resolution = e.NewResolution;
        Resolution virtualResolution = ResolutionManager.VirtualResolution;

        CalculateRenderBounds(resolution, virtualResolution);
    }

    private void OnVirtualResolutionChanged(object? sender, ResolutionChangedArgs e)
    {
        Resolution virtualResolution = e.NewResolution;
        if (_renderTarget != null && _renderTarget.Width == virtualResolution.Width && _renderTarget.Height == virtualResolution.Height)
            return;

        _renderTarget?.Dispose();
        _renderTarget = new RenderTarget2D(GraphicsDevice, virtualResolution.Width, virtualResolution.Height);
    }

    private void CalculateRenderBounds(Resolution resolution, Resolution virtualResolution)
    {
        float scaleX = (float)resolution.Width / virtualResolution.Width;
        float scaleY = (float)resolution.Height / virtualResolution.Height;
        float scale = MathF.Min(scaleX, scaleY);

        int width = (int)(virtualResolution.Width * scale);
        int height = (int)(virtualResolution.Height * scale);
        int x = (resolution.Width - width) / 2;
        int y = (resolution.Height - height) / 2;

        _renderBounds = new Rectangle(x, y, width, height);
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

    public void DoInitialize()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Initialize();
    }

    public void DoLoad()
    {
        Resolution resolution = ResolutionManager.VirtualResolution;
        _renderTarget = new RenderTarget2D(GraphicsDevice, resolution.Width, resolution.Height);

        Load();
    }

    public void DoUnload()
    {
        Unload();

        ResolutionManager.ResolutionChanged -= OnResolutionChanged;
        ResolutionManager.VirtualResolutionChanged -= OnVirtualResolutionChanged;
        _renderTarget?.Dispose();
    }

    public void DoUpdate(GameTime gameTime)
    {
        Update(gameTime);
    }

    public void DoDraw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(_renderTarget);
        Draw(gameTime);
        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_renderTarget, RenderBounds, Color.White);
        _spriteBatch.Draw(Engine.AssetsManager.Sprite_Blank.Texture, RenderBounds, Color.Black * _transitionOpacity);
        _spriteBatch.End();
    }

    protected virtual void Initialize()
    {
    }

    protected virtual void Load()
    {
    }

    protected virtual void Unload()
    {
    }

    protected virtual void Update(GameTime gameTime)
    {
    }

    protected virtual void Draw(GameTime gameTime)
    {
    }

    protected T GetEngine<T>() where T : PokesharpEngine
    {
        return (T)Engine;
    }
}